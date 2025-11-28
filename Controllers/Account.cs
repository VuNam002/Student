using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.Services;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Account : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<Account> _logger;
        public Account(IAccountService accountService, ILogger<Account> logger)
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
                
                if(token == null)
                {
                    return Unauthorized("Invalid username or password");
                }
                return Ok(token);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
