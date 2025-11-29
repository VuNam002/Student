using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.Services;

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
                    return Unauthorized("Invalid username or password");
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
    }
}
