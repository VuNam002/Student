using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs; // Giả sử bạn có thư mục DTOs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_management.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts
                .Include(a => a.Role) 
                .Select(a => new AccountDto
                {
                    ID = a.ID,
                    TenDangNhap = a.TenDangNhap,
                    VaiTroID = a.Role.TenVaiTro
                })
                .ToListAsync();

            return accounts;
        }
    }
}