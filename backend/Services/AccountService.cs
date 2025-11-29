using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Student_management.Data;
using Student_management.DTOs.Account;
using Student_management.Models;
using Student_management.Helpers; 
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
                    .Include(a => a.Role)
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        Email = a.Email,
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
                _logger.LogError(ex, "Loi khi lay toan bo danh sach tai khoan");
                throw;
            }
        }

        public async Task<string?> LoginAsync(LoginRequestDto loginRequest)
        {
            if (loginRequest is null) return null;

            if (string.IsNullOrEmpty(loginRequest.MatKhau) || loginRequest.MatKhau.Length < 6)
            {
                _logger.LogWarning("Login attempt for {User} with too short password.", loginRequest.Email);
                return null;
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == loginRequest.Email);

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

            // Sử dụng HashHelper để verify password
            if (!HashHelper.VerifyPassword(loginRequest.MatKhau, storedPassword))
            {
                _logger.LogWarning("Login attempt failed for user {User} due to incorrect password.", loginRequest.Email);
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
                    new Claim(ClaimTypes.Name, account.Email ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<AccountDto?> Detail(int id)
        {
            try
            {
                var account = await _context.Accounts
                    .Include(a => a.Role)
                    .Where(a => a.AccountID == id)
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        Email = a.Email,
                        RoleID = a.RoleID.ToString(),
                        TenHienThi = a.Role != null ? a.Role.TenHienThi : null,
                        Avatar = a.Avatar,
                        TrangThai = a.TrangThai,
                        NgayTao = a.NgayTao
                    }).FirstOrDefaultAsync();

                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay chi tiet tai khoan ID: {AccountId}", id);
                throw;
            }
        }

        public async Task<AccountDto> CreateAccount(CreateAccount dto)
        {
            try
            {
                var username = dto.Email?.Trim();
                if (string.IsNullOrWhiteSpace(username))
                    throw new InvalidOperationException("TenDangNhap is required.");

                if (string.IsNullOrWhiteSpace(dto.MatKhau) || dto.MatKhau.Length < 6)
                    throw new InvalidOperationException("MatKhau is required and must be at least 6 characters.");

                var exists = await _context.Accounts.AnyAsync(a => a.Email == username);
                if (exists)
                    throw new InvalidOperationException($"Username '{username}' already exists.");

                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
                if (!roleExists)
                    throw new InvalidOperationException($"Role with ID {dto.RoleID} does not exist.");

                var newAccount = new Account
                {
                    Email = username,
                    RoleID = dto.RoleID,
                    Avatar = dto.Avatar,
                    TrangThai = dto.TrangThai,
                    NgayTao = dto.NgayTao
                };

                // Sử dụng HashHelper để hash password
                newAccount.MatKhau = HashHelper.ComputeMd5Hash(dto.MatKhau);

                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                return new AccountDto
                {
                    ID = newAccount.AccountID,
                    Email = newAccount.Email,
                    RoleID = newAccount.RoleID.ToString(),
                    Avatar = newAccount.Avatar,
                    TrangThai = newAccount.TrangThai,
                    NgayTao = newAccount.NgayTao,
                    TenHienThi = await _context.Roles
                        .Where(r => r.RoleID == newAccount.RoleID)
                        .Select(r => r.TenHienThi)
                        .FirstOrDefaultAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi tao tai khoan moi");
                throw;
            }
        }

        public async Task<AccountDto> EditAccount(int id, CreateAccount dto)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                    throw new KeyNotFoundException($"Tai khoan moi voi ID {id} khong ton tai.");

                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
                if (!roleExists)
                    throw new InvalidOperationException($"Role voi ID {dto.RoleID} khong ton tai.");

                account.Email = dto.Email ?? account.Email;

                // Sử dụng HashHelper để hash password mới
                if (!string.IsNullOrWhiteSpace(dto.MatKhau))
                {
                    if (dto.MatKhau.Length < 6)
                        throw new InvalidOperationException("MatKhau phai co it nhat 6 ky tu.");
                    account.MatKhau = HashHelper.ComputeMd5Hash(dto.MatKhau);
                }

                account.RoleID = dto.RoleID;
                account.Avatar = dto.Avatar ?? account.Avatar;
                account.TrangThai = dto.TrangThai;
                if (dto.NgayTao != default) account.NgayTao = dto.NgayTao;

                await _context.SaveChangesAsync();

                var tenHienThi = await _context.Roles
                    .Where(r => r.RoleID == account.RoleID)
                    .Select(r => r.TenHienThi)
                    .FirstOrDefaultAsync();

                return new AccountDto
                {
                    ID = account.AccountID,
                    Email = account.Email,
                    RoleID = account.RoleID.ToString(),
                    Avatar = account.Avatar,
                    TrangThai = account.TrangThai,
                    NgayTao = account.NgayTao,
                    TenHienThi = tenHienThi
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi sua tai khoan ID: {AccountId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAccount(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                    return false;

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi xoa tai khoan ID: {AccountId}", id);
                throw;
            }
        }
        public async Task<Pagination> GetAccountPagination(AccountSearch searchParams)
        {
            try
            {
                var query = _context.Accounts.AsNoTracking().Include(a => a.Role).AsQueryable();

                // Lọc theo từ khóa
                if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(a => a.Email!.ToLower().Contains(keyword) ||
                                             (a.Role != null && a.Role.TenHienThi!.ToLower().Contains(keyword)));
                }

                // Lọc theo trạng thái
                if (searchParams.TrangThai.HasValue)
                {
                    query = query.Where(a => a.TrangThai == searchParams.TrangThai.Value);
                }

                // Tổng số bản ghi
                var totalCount = await query.CountAsync();

                // Lấy dữ liệu phân trang
                var accounts = await query
                    .Skip((searchParams.Page - 1) * searchParams.PageSize)
                    .Take(searchParams.PageSize)
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        Email = a.Email,
                        RoleID = a.RoleID.ToString(),
                        TenHienThi = a.Role != null ? a.Role.TenHienThi : null,
                        Avatar = a.Avatar,
                        TrangThai = a.TrangThai,
                        NgayTao = a.NgayTao
                    })
                    .ToListAsync();

                // Tính tổng số trang
                var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

                return new Pagination
                {
                    Account = accounts,
                    TotalCount = totalCount,
                    Page = searchParams.Page,
                    PageSize = searchParams.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm tài khoản");
                throw;
            }
        }
    }
}