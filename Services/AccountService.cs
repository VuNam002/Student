using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs; // Giả sử bạn có thư mục DTOs
using Student_management.DTOs.Account;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Cần thiết cho phương thức Select()
using Microsoft.Extensions.Logging; // Cần thiết cho ILogger
using Microsoft.Extensions.Configuration; // Cần thiết cho IConfiguration
using System; // Cần thiết cho Exception

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