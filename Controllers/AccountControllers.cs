using Microsoft.AspNetCore.Mvc;
using Student_management.Services;
using Student_management.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Student_management.DTOs.Account;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        // Inject IAccountService thông qua constructor
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Endpoint để lấy tất cả tài khoản
        // GET: api/account
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Services.AccountDto>>> GetAccounts()
        {
            // Gọi phương thức từ service
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }
    }
}
