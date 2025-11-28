using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Student_management.Data;
using Student_management.DTOs.Account;
using Student_management.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks; 

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
                    .AsNoTracking()
                    .Include(a => a.Role) // Role
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        TenDangNhap = a.TenDangNhap,
                        RoleID = a.RoleID.ToString(),
                        TenHienThi = a.Role != null ? a.Role.TenHienThi : null, 
                        Avatar = a.Avatar,
                        TrangThai = a.TrangThai,
                        NgayTao = a.NgayTao
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ danh sách tài khoản");
                throw;
            }
        }

        public async Task<string?> LoginAsync(LoginRequestDto loginRequest)
        {
            if (loginRequest is null) return null;

            if (string.IsNullOrEmpty(loginRequest.MatKhau) || loginRequest.MatKhau.Length < 6)
            {
                _logger.LogWarning("Login attempt for {User} with too short password.", loginRequest.TenDangNhap);
                return null;
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.TenDangNhap == loginRequest.TenDangNhap);

            if (account is null || account.TrangThai != true)
            {
                return null; 
            }

            var storedPassword = account.MatKhau;

            if (string.IsNullOrWhiteSpace(storedPassword))
            {
                _logger.LogWarning("Account {AccountId} has empty stored password.", account.AccountID);
                return null;
            }

            if (loginRequest.MatKhau != storedPassword)
            {
                _logger.LogWarning("Login attempt failed for user {User} due to incorrect password.", loginRequest.TenDangNhap);
                return null;
            }
            return GenerateJwtToken(account);
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyString))
                throw new InvalidOperationException("JWT Key is not configured.");

            var key = Encoding.ASCII.GetBytes(keyString);

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountID.ToString()),
                    new Claim(ClaimTypes.Name, account.TenDangNhap ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}