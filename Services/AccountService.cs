using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Account;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; 
using Microsoft.Extensions.Logging; 
using Microsoft.Extensions.Configuration; 
using System; 

namespace Student_management.Services
{
    public class AccountService : IAccountService 
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountService> _logger;
        private readonly IConfiguration _configuration;

        public AccountService(AppDbContext context, ILogger<AccountService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<AccountDto>> GetAll()
        {
            try
            {

                return await _context.Accounts
                    .Select(a => new AccountDto 
                    {
                        ID = a.ID,
                        TenDangNhap = a.TenDangNhap,
                    })
                    .ToListAsync(); 

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ danh sách tài khoản");
                throw; 
            }
        }
    }
}