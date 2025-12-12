using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.Enum;
using Student_management.Services.Interfaces;
using System.Security.Claims;
using System;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<CreateAccount> _createAccountValidator;

        public AccountController(
            IAccountService accountService,
            ILogger<AccountController> logger,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<CreateAccount> createAccountValidator)
        {
            _accountService = accountService;
            _logger = logger;
            _loginValidator = loginValidator;
            _createAccountValidator = createAccountValidator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAll()
        {
            try
            {
                var accounts = await _accountService.GetAll();
                return Ok(new { success = true, data = accounts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all accounts.");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string?>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var validationResult = await _loginValidator.ValidateAsync(loginRequest);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = validationResult.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                }

                var token = await _accountService.LoginAsync(loginRequest);

                if (token == null)
                {
                    _logger.LogWarning("Login failed for user {Email}. Invalid credentials or inactive account.", loginRequest.Email);
                    return Unauthorized(new { success = false, message = "Email hoac mat khau khong dung" });
                }

                return Ok(new { success = true, token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> Detail(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID phai lon hon 0" });
                }

                var account = await _accountService.Detail(id);
                if (account == null)
                {
                    return NotFound(new { success = false, message = "Khong tim thay tai khoan" });
                }

                return Ok(new { success = true, data = account });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting account by ID: {AccountId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccount dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { success = false, message = "Du lieu khong hop le" });
                }

                var validationResult = await _createAccountValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = validationResult.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                }

                var createdAccount = await _accountService.CreateAccount(dto);
                return CreatedAtAction(
                    nameof(Detail),
                    new { id = createdAccount.ID },
                    new { success = true, data = createdAccount }
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating account.");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an account.");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> EditAccount(int id, [FromBody] CreateAccount dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID phải lớn hơn 0" });
                }

                if (dto == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var validationResult = await _createAccountValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Where(e => !(e.PropertyName == "Password" && string.IsNullOrWhiteSpace(dto.Password)))
                        .ToList();

                    if (errors.Any())
                    {
                        return BadRequest(new
                        {
                            success = false,
                            errors = errors.Select(e => new
                            {
                                field = e.PropertyName,
                                message = e.ErrorMessage
                            })
                        });
                    }
                }

                var updatedAccount = await _accountService.EditAccount(id, dto);
                return Ok(new { success = true, data = updatedAccount });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while editing account ID: {AccountId}", id);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found while editing ID: {AccountId}", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing account ID: {AccountId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID phai lon hon 0" });
                }

                var result = await _accountService.DeleteAccount(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "khong tim thay tai khoan de xoa" });
                }

                return Ok(new { success = true, message = "Xoa tai khoan thanh cong" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting account ID: {AccountId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<Pagination>> GetAccountPagination([FromQuery] AccountSearch accountSearch)
        {
            try
            {
                if (accountSearch.Page <= 0 || accountSearch.PageSize <= 0)
                {
                    return BadRequest(new { success = false, message = "Page va PageSize phai lon hon 0" });
                }

                if (accountSearch.PageSize > 100)
                {
                    return BadRequest(new { success = false, message = "PageSize khong vuot qua 100" });
                }

                var paginatedAccounts = await _accountService.GetAccountPagination(accountSearch);
                return Ok(new { success = true, data = paginatedAccounts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting paginated account list.");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateAccountStatus(int id, [FromBody] AccountStatusRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID phai lon hon 0" });
                }

                if (!System.Enum.IsDefined(typeof(AccountStatus), request.Status))
                {
                    return BadRequest(new { success = false, message = "Trang thai khong hop le" });
                }

                var result = await _accountService.UpdateAccountStatus(id, (AccountStatus)request.Status);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Khong tim thay tai khoan" });
                }

                return Ok(new { success = true, message = "Cap nhat trang thai thanh cong" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating account status for ID: {AccountId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<AccountDto>> GetMyAccount()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
                {
                    return Unauthorized(new { success = false, message = "Khong the xac thuc tai khoan" });
                }

                var account = await _accountService.Detail(accountId);
                if (account == null)
                {
                    return NotFound(new { success = false, message = "Khong tim thay tai khoan" });
                }

                return Ok(new { success = true, data = account });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting my account information.");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { success = false, message = "Token khong ton tai" });
                }

                await _accountService.LogoutAsync(token);

                return Ok(new { success = true, message = "Dang xuat thanh cong" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout.");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }

    // DTO cho UpdateAccountStatus
    public class AccountStatusRequest
    {
        public byte Status { get; set; }
    }
}