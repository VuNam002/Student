using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.Services;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountControllers: ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly ILogger<AccountControllers> _logger;
        public AccountControllers(AccountService accountService, ILogger<AccountControllers> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAll()
        {

        }
    }
}
