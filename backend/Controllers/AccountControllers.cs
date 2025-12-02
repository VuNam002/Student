using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.Enum;
using Student_management.Services;
using System.Security.Claims;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAll()
        {
            try
            {
                var accounts = await _accountService.GetAll();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all accounts.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string?>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var token = await _accountService.LoginAsync(loginRequest);

                if (token == null)
                {
                    _logger.LogWarning("Login failed for user {Email}. Invalid credentials or inactive account.", loginRequest.Email);
                    return Unauthorized("Tên đăng nhập hoặc mật khẩu không hợp lệ.");
                }
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> Detail(int id)
        {
            try
            {
                var account = await _accountService.Detail(id);
                if (account == null)
                {
                    return NotFound("Khong tim thay tai khoan");
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting account by ID.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccount dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Du lieu nguoi dung khong hop le");
                }

                var account = new CreateAccount
                {
                    Email = dto.Email,
                    MatKhau = dto.MatKhau,
                    RoleID = dto.RoleID,
                    Avatar = dto.Avatar,
                    TrangThai = dto.TrangThai,
                    NgayTao = dto.NgayTao,
                    HoTen = dto.HoTen,
                    SDT = dto.SDT
                };

                var createdAccount = await _accountService.CreateAccount(account);
                return Ok(createdAccount);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating account.");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an account.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> EditAccount(int id, [FromBody] CreateAccount dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Du lieu nguoi dung khong hop le");
                }

                var account = new CreateAccount
                {
                    Email = dto.Email,
                    MatKhau = dto.MatKhau,
                    RoleID = dto.RoleID,
                    Avatar = dto.Avatar,
                    TrangThai = dto.TrangThai,
                    NgayTao = dto.NgayTao,
                    HoTen = dto.HoTen,
                    SDT = dto.SDT
                };

                var updatedAccount = await _accountService.EditAccount(id, account);
                return Ok(updatedAccount);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while editing account.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found while editing.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing an account.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var result = await _accountService.DeleteAccount(id);
                if (!result)
                {
                    return NotFound("Khong tim thay tai khoan de xoa");
                }
                return Ok(new { message = "Xoa tai khoan thanh cong" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting an account.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("paginated")]
        public async Task<ActionResult<Pagination>> GetAccountPagination([FromQuery] AccountSearch accountSearch)
        {
            try
            {
                // Xác thực giá trị Page và PageSize
                if (accountSearch.Page <= 0 || accountSearch.PageSize <= 0)
                {
                    return BadRequest("Page và PageSize phải lớn hơn 0.");
                }

                Pagination paginatedAccounts = await _accountService.GetAccountPagination(accountSearch);
                return Ok(paginatedAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy danh sách tài khoản phân trang.");
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
        //cap nhat trang thai san pham
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateAccountStatus(int id, [FromBody] byte trangThai)
        {
            try
            {
                var updatedAccount = await _accountService.UpdateAccountStatus(id, (AccountStatus)trangThai);
                return Ok(updatedAccount);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found while updating status.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating account status.");
                return StatusCode(500, "Internal server error");
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
                    return Unauthorized("Không thể xác thực tài khoản.");
                }

                var account = await _accountService.Detail(accountId);
                if (account == null)
                {
                    return NotFound("Không tìm thấy tài khoản.");
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy thông tin tài khoản của tôi.");
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Extract the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token not found.");
                }

                await _accountService.LogoutAsync(token);

                return NoContent(); // Or Ok("Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
