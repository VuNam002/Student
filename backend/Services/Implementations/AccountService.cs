using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Student_management.Data;
using Student_management.DTOs.Account;
using Student_management.DTOs.Permission;
using Student_management.Enum;
using Student_management.Helpers;
using Student_management.Models;
using Student_management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Student_management.Services.Implementations
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
                    .AsNoTracking()//tat co che theo doi
                    .Where(a => !a.IsDeleted == true)
                    .Include(a => a.Role)
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        Email = a.Email,
                        RoleID = a.RoleID.ToString(),
                        RoleName = a.Role != null ? a.Role.RoleName : null,
                        Avatar = a.Avatar,
                        Status = a.Status,
                        FullName = a.FullName,
                        PhoneNumber = a.PhoneNumber,
                        CreatedAt = a.CreatedAt,
                        IsDeleted = a.IsDeleted,
                        UpdatedAt = a.UpdatedAt
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
 
            if (string.IsNullOrEmpty(loginRequest.Email))
            {
                _logger.LogWarning("Login attempt with empty email.");
                return null;
            }

            if (string.IsNullOrEmpty(loginRequest.Password))
            {
                _logger.LogWarning("Login attempt for {User} with empty password.", loginRequest.Email);
                return null;
            }

            var email = loginRequest.Email.Trim();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());

            if (account is null)
            {
                _logger.LogWarning("Login failed: Account not found for email {Email}", email);
                return null;
            }

            if (account.Status != 1)
            {
                _logger.LogWarning("Login failed: Account {Email} is inactive. Status: {Status}", email, account.Status);
                return null;
            }

            var storedPassword = account.Password;

            if (string.IsNullOrWhiteSpace(storedPassword))
            {
                _logger.LogWarning("Account {AccountId} has empty stored password.", account.AccountID);
                return null;
            }

           
            var inputPasswordHash = HashHelper.ComputeMd5Hash(loginRequest.Password);

            if (storedPassword != inputPasswordHash)
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

            var key = Encoding.UTF8.GetBytes(keyString);

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountID.ToString()), 
                    new Claim(ClaimTypes.Name, account.Email ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
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
                    .Where(a => a.AccountID == id && a.IsDeleted == false)
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        Email = a.Email,
                        RoleID = a.RoleID.ToString(),
                        RoleName = a.Role != null ? a.Role.RoleName : null,
                        Avatar = a.Avatar,
                        FullName = a.FullName,
                        PhoneNumber = a.PhoneNumber,
                        Status = a.Status,
                        CreatedAt = a.CreatedAt
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

                if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
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
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Status = (byte)AccountStatus.Active,
                    CreatedAt = dto.CreatedAt
                };

                newAccount.Password = HashHelper.ComputeMd5Hash(dto.Password);

                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                return new AccountDto
                {
                    ID = newAccount.AccountID,
                    Email = newAccount.Email,
                    RoleID = newAccount.RoleID.ToString(),
                    Avatar = newAccount.Avatar,
                    FullName = newAccount.FullName,
                    PhoneNumber = newAccount.PhoneNumber,
                    Status = newAccount.Status,
                    CreatedAt = newAccount.CreatedAt,
                    RoleName = await _context.Roles
                        .Where(r => r.RoleID == newAccount.RoleID)
                        .Select(r => r.RoleName)
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
                var account = await _context.Accounts.Where(a => a.AccountID == id && a.IsDeleted == false).FirstOrDefaultAsync();
                if (account == null)
                    throw new KeyNotFoundException($"Tai khoan moi voi ID {id} khong ton tai.");

                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
                if (!roleExists)
                    throw new InvalidOperationException($"Role voi ID {dto.RoleID} khong ton tai.");
                if(!string.IsNullOrEmpty(dto.Email))
                {
                    var emailCheck = dto.Email.Trim().ToLower();
                    var isDuplicate = await _context.Accounts.AnyAsync(a => a.Email.ToLower() == emailCheck && a.AccountID != id && !a.IsDeleted);
                    if(isDuplicate)
                    {
                        throw new ArgumentException("{\"success\":false,\"errors\":[{\"field\":\"Email\",\"message\":\"Email da ton tai\"}]}");
                    }
                }

                account.Email = dto.Email ?? account.Email;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    if (dto.Password.Length < 6)
                        throw new InvalidOperationException("Mat khau phai co it nhat 6 ky tu.");
                    account.Password = HashHelper.ComputeMd5Hash(dto.Password);
                }

                account.RoleID = dto.RoleID;
                account.Avatar = dto.Avatar ?? account.Avatar;
                account.Status = dto.Status;
                account.FullName = dto.FullName ?? account.FullName;
                account.PhoneNumber = dto.PhoneNumber ?? account.PhoneNumber;
                account.UpdatedAt = DateTime.Now;
                if (dto.CreatedAt != default) account.CreatedAt = dto.CreatedAt;

                await _context.SaveChangesAsync();

                var roleName = await _context.Roles
                    .Where(r => r.RoleID == account.RoleID)
                    .Select(r => r.RoleName)
                    .FirstOrDefaultAsync();

                return new AccountDto
                {
                    ID = account.AccountID,
                    Email = account.Email,
                    RoleID = account.RoleID.ToString(),
                    Avatar = account.Avatar,
                    Status = account.Status,
                    CreatedAt = account.CreatedAt,
                    RoleName = roleName,
                    FullName = account.FullName,
                    PhoneNumber = account.PhoneNumber
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
                if (account == null|| account.IsDeleted)
                {
                    return false;
                }
                account.IsDeleted = true;
                account.UpdatedAt = DateTime.Now;
                _context.Accounts.Update(account);
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
                var query = _context.Accounts.AsNoTracking().Include(a => a.Role).Where(a => a.IsDeleted == false).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(a => a.Email!.ToLower().Contains(keyword) ||
                                             (a.Role != null && a.Role.RoleName!.ToLower().Contains(keyword)));
                }

                if (searchParams.Status.HasValue)
                {
                    query = query.Where(a => a.Status == searchParams.Status.Value);
                }

                var totalCount = await query.CountAsync();

                var accounts = await query
                    .Skip((searchParams.Page - 1) * searchParams.PageSize)
                    .Take(searchParams.PageSize)
                    .Select(a => new AccountDto
                    {
                        ID = a.AccountID,
                        Email = a.Email,
                        RoleID = a.RoleID.ToString(),
                        RoleName = a.Role != null ? a.Role.RoleName : null,
                        Avatar = a.Avatar,
                        FullName = a.FullName,
                        PhoneNumber = a.PhoneNumber,
                        Status = a.Status,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

               //Pagination
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
                _logger.LogError(ex, "Loi khi tim kiem tai khoan");
                throw;
            }
        }

        public async Task<bool> UpdateAccountStatus(int id, AccountStatus Status)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null)
                    return false;

                account.Status = (byte)Status;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi cap nhat trang thai tai khoan ID: {AccountId}", id);
                throw;
            }
        }

        public async Task LogoutAsync(string token)
        {
            await Task.CompletedTask;
        }

        public Task<List<PermissionDto>> GetAllPermissions(string? module = null)
        {
            throw new NotImplementedException();
        }
    }
}