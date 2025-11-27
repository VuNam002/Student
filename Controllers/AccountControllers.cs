using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.Services;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountControllers : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountControllers> _logger;
        public AccountControllers(IAccountService accountService, ILogger<AccountControllers> logger)
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
    }
}
