using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using Student_management.Models.Entities;
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
        private readonly IMapper _mapper;

        public AccountService(
            AppDbContext context,
            ILogger<AccountService> logger,
            IConfiguration configuration,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<List<AccountDto>> GetAll()
        {
            try
            {
                var accounts = await _context.Accounts
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .Include(a => a.Role)
                    .ToListAsync();

                return _mapper.Map<List<AccountDto>>(accounts);
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
                    .FirstOrDefaultAsync();

                if (account == null)
                    return null;

                var accountDto = _mapper.Map<AccountDto>(account);

                // Find Person by Email
                var person = await _context.Persons
                    .Where(p => p.Email == account.Email && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (person != null)
                {
                    // Map Person data
                    accountDto.DateOfBirth = person.DateOfBirth;
                    accountDto.Gender = person.Gender;
                    accountDto.Address = person.Address;
                    accountDto.IdentityCard = person.IdentityCard;

                    // Find Teacher if exists
                    var teacher = await _context.Teachers
                        .Where(t => t.AccountID == account.AccountID && !t.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (teacher != null)
                    {
                        accountDto.DepartmentID = teacher.DepartmentID;
                        accountDto.Position = teacher.Position;
                        accountDto.Degree = teacher.Degree;
                        accountDto.Specialization = teacher.Specialization;
                    }

                    // Find Student if exists
                    var student = await _context.Students
                        .Where(s => s.AccountID == account.AccountID && !s.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (student != null)
                    {
                        accountDto.ClassID = student.ClassID;
                        accountDto.EnrollmentDate = student.EnrollmentDate;
                    }
                }

                return accountDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay chi tiet tai khoan ID: {AccountId}", id);
                throw;
            }
        }

        public async Task<AccountDto> CreateAccount(CreateAccount dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var username = dto.Email?.Trim();
                if (string.IsNullOrWhiteSpace(username))
                    throw new InvalidOperationException("Email is required.");

                if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                    throw new InvalidOperationException("Password is required and must be at least 6 characters.");

                if (string.IsNullOrWhiteSpace(dto.FullName))
                    throw new InvalidOperationException("FullName is required.");

                var exists = await _context.Accounts.AnyAsync(a => a.Email == username);
                if (exists)
                    throw new InvalidOperationException($"Email '{username}' already exists.");

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == dto.RoleID);
                if (role == null)
                    throw new InvalidOperationException($"Role with ID {dto.RoleID} does not exist.");

                var newPerson = new Person
                {
                    PersonType = role.RoleName ?? "Other",
                    FullName = dto.FullName,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    IdentityCard = dto.IdentityCard,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Persons.Add(newPerson);
                await _context.SaveChangesAsync();

                _logger.LogInformation($" Created Person ID: {newPerson.PersonID}");
                var newAccount = new Account
                {
                    Email = dto.Email,
                    Password = HashHelper.ComputeMd5Hash(dto.Password),
                    RoleID = dto.RoleID,
                    Avatar = dto.Avatar,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Status = dto.Status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                _logger.LogInformation($" Created Account ID: {newAccount.AccountID}");

                var roleName = role.RoleName?.ToLower().Trim();

                if (roleName == "teacher" || roleName == "giáo viên" || roleName == "gv")
                {
                    if (!dto.DepartmentID.HasValue)
                        throw new InvalidOperationException("DepartmentID is required for Teacher role.");

                    var departmentExists = await _context.Departments
                        .AnyAsync(d => d.DepartmentID == dto.DepartmentID.Value && !d.IsDeleted);
                    if (!departmentExists)
                        throw new InvalidOperationException($"Department with ID {dto.DepartmentID} does not exist.");

                    var teacher = new Teacher
                    {
                        PersonID = newPerson.PersonID,
                        TeacherCode = await GenerateTeacherCode(),
                        DepartmentID = dto.DepartmentID.Value,
                        Position = dto.Position ?? "Giảng viên",
                        Degree = dto.Degree,
                        Specialization = dto.Specialization,
                        AccountID = newAccount.AccountID,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($" Created Teacher ID: {teacher.TeacherID}, Code: {teacher.TeacherCode}");
                }
                else if (roleName == "student" || roleName == "sinh viên" || roleName == "sv")
                {
                    if (!dto.ClassID.HasValue)
                        throw new InvalidOperationException("ClassID is required for Student role.");

                    var classExists = await _context.Classes
                        .AnyAsync(c => c.ClassID == dto.ClassID.Value && !c.IsDeleted);
                    if (!classExists)
                        throw new InvalidOperationException($"Class with ID {dto.ClassID} does not exist.");

                    var student = new Student
                    {
                        PersonID = newPerson.PersonID,
                        StudentCode = await GenerateStudentCode(),
                        ClassID = dto.ClassID.Value,
                        EnrollmentDate = dto.EnrollmentDate ?? DateTime.Now,
                        Status = StudentStatus.Active,
                        AccountID = newAccount.AccountID,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($" Created Student ID: {student.StudentID}, Code: {student.StudentCode}");
                }
                else
                {
                    _logger.LogWarning($"Role '{roleName}' is neither Teacher nor Student.");
                }
                await transaction.CommitAsync();
                _logger.LogInformation($" Successfully created account for: {dto.FullName}");

                var accountWithRole = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstAsync(a => a.AccountID == newAccount.AccountID);

                return _mapper.Map<AccountDto>(accountWithRole);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, " Error creating account. Rolled back.");
                throw;
            }
        }

        public async Task<AccountDto> EditAccount(int id, CreateAccount dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var account = await _context.Accounts
                    .Where(a => a.AccountID == id && a.IsDeleted == false)
                    .FirstOrDefaultAsync();

                if (account == null)
                    throw new KeyNotFoundException($"Tai khoan voi ID {id} khong ton tai.");

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == dto.RoleID);
                if (role == null)
                    throw new InvalidOperationException($"Role voi ID {dto.RoleID} khong ton tai.");

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var emailCheck = dto.Email.Trim().ToLower();
                    var isDuplicate = await _context.Accounts
                        .AnyAsync(a => a.Email.ToLower() == emailCheck && a.AccountID != id && !a.IsDeleted);

                    if (isDuplicate)
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

                await _context.SaveChangesAsync();
                _logger.LogInformation($"✓ Updated Account ID: {account.AccountID}");

                var person = await _context.Persons
                    .FirstOrDefaultAsync(p => p.Email == account.Email && !p.IsDeleted);

                if (person == null)
                {
                    person = new Person
                    {
                        PersonType = role.RoleName ?? "Other",
                        FullName = dto.FullName,
                        DateOfBirth = dto.DateOfBirth,
                        Gender = dto.Gender,
                        Email = dto.Email,
                        PhoneNumber = dto.PhoneNumber,
                        Address = dto.Address,
                        IdentityCard = dto.IdentityCard,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Persons.Add(person);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"✓ Created Person ID: {person.PersonID}");
                }
                else
                {
                    person.PersonType = role.RoleName ?? person.PersonType;
                    person.FullName = dto.FullName ?? person.FullName;
                    person.DateOfBirth = dto.DateOfBirth ?? person.DateOfBirth;
                    person.Gender = dto.Gender ?? person.Gender;
                    person.PhoneNumber = dto.PhoneNumber ?? person.PhoneNumber;
                    person.Address = dto.Address ?? person.Address;
                    person.IdentityCard = dto.IdentityCard ?? person.IdentityCard;
                    person.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"✓ Updated Person ID: {person.PersonID}");
                }

                var roleName = role.RoleName?.ToLower().Trim();
                if (roleName == "teacher" || roleName == "giáo viên" || roleName == "gv")
                {
                    var teacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.AccountID == account.AccountID && !t.IsDeleted);

                    if (teacher == null)
                    {
                        if (!dto.DepartmentID.HasValue)
                            throw new InvalidOperationException("DepartmentID is required for Teacher role.");

                        var departmentExists = await _context.Departments
                            .AnyAsync(d => d.DepartmentID == dto.DepartmentID.Value && !d.IsDeleted);
                        if (!departmentExists)
                            throw new InvalidOperationException($"Department with ID {dto.DepartmentID} does not exist.");

                        teacher = new Teacher
                        {
                            PersonID = person.PersonID,
                            TeacherCode = await GenerateTeacherCode(),
                            DepartmentID = dto.DepartmentID.Value,
                            Position = dto.Position ?? "Giảng viên",
                            Degree = dto.Degree,
                            Specialization = dto.Specialization,
                            AccountID = account.AccountID,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.Teachers.Add(teacher);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($" Created Teacher ID: {teacher.TeacherID}");
                    }
                    else
                    {
                        if (dto.DepartmentID.HasValue)
                        {
                            var departmentExists = await _context.Departments
                                .AnyAsync(d => d.DepartmentID == dto.DepartmentID.Value && !d.IsDeleted);
                            if (!departmentExists)
                                throw new InvalidOperationException($"Department with ID {dto.DepartmentID} does not exist.");

                            teacher.DepartmentID = dto.DepartmentID.Value;
                        }
                        teacher.Position = dto.Position ?? teacher.Position;
                        teacher.Degree = dto.Degree ?? teacher.Degree;
                        teacher.Specialization = dto.Specialization ?? teacher.Specialization;
                        teacher.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Updated Teacher ID: {teacher.TeacherID}");
                    }

                    var existingStudent = await _context.Students
                        .FirstOrDefaultAsync(s => s.AccountID == account.AccountID && !s.IsDeleted);
                    if (existingStudent != null)
                    {
                        existingStudent.IsDeleted = true;
                        existingStudent.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"✓ Soft-deleted Student ID: {existingStudent.StudentID}");
                    }
                }
                else if (roleName == "student" || roleName == "sinh viên" || roleName == "sv")
                {
                    var student = await _context.Students
                        .FirstOrDefaultAsync(s => s.AccountID == account.AccountID && !s.IsDeleted);

                    if (student == null)
                    {
                        if (!dto.ClassID.HasValue)
                            throw new InvalidOperationException("ClassID is required for Student role.");

                        var classExists = await _context.Classes
                            .AnyAsync(c => c.ClassID == dto.ClassID.Value && !c.IsDeleted);
                        if (!classExists)
                            throw new InvalidOperationException($"Class with ID {dto.ClassID} does not exist.");

                        student = new Student
                        {
                            PersonID = person.PersonID,
                            StudentCode = await GenerateStudentCode(),
                            ClassID = dto.ClassID.Value,
                            EnrollmentDate = dto.EnrollmentDate ?? DateTime.Now,
                            Status = StudentStatus.Active,
                            AccountID = account.AccountID,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.Students.Add(student);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"✓ Created Student ID: {student.StudentID}");
                    }
                    else
                    {
                        if (dto.ClassID.HasValue)
                        {
                            var classExists = await _context.Classes
                                .AnyAsync(c => c.ClassID == dto.ClassID.Value && !c.IsDeleted);
                            if (!classExists)
                                throw new InvalidOperationException($"Class with ID {dto.ClassID} does not exist.");

                            student.ClassID = dto.ClassID.Value;
                        }
                        student.EnrollmentDate = dto.EnrollmentDate ?? student.EnrollmentDate;
                        student.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"✓ Updated Student ID: {student.StudentID}");
                    }

                    var existingTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.AccountID == account.AccountID && !t.IsDeleted);
                    if (existingTeacher != null)
                    {
                        existingTeacher.IsDeleted = true;
                        existingTeacher.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($" Soft-deleted Teacher ID: {existingTeacher.TeacherID}");
                    }
                }
                else
                {
                    var existingTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.AccountID == account.AccountID && !t.IsDeleted);
                    if (existingTeacher != null)
                    {
                        existingTeacher.IsDeleted = true;
                        existingTeacher.UpdatedAt = DateTime.Now;
                        _logger.LogInformation($" Soft-deleted Teacher ID: {existingTeacher.TeacherID}");
                    }

                    var existingStudent = await _context.Students
                        .FirstOrDefaultAsync(s => s.AccountID == account.AccountID && !s.IsDeleted);
                    if (existingStudent != null)
                    {
                        existingStudent.IsDeleted = true;
                        existingStudent.UpdatedAt = DateTime.Now;
                        _logger.LogInformation($" Soft-deleted Student ID: {existingStudent.StudentID}");
                    }

                    if (existingTeacher != null || existingStudent != null)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                _logger.LogInformation($"Successfully updated account ID: {account.AccountID}");

                var accountWithRole = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstAsync(a => a.AccountID == account.AccountID);

                return _mapper.Map<AccountDto>(accountWithRole);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error updating account ID: {AccountId}. Rolled back.", id);
                throw;
            }
        }

        public async Task<bool> DeleteAccount(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);
                if (account == null || account.IsDeleted)
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
                var query = _context.Accounts
                    .AsNoTracking()
                    .Include(a => a.Role)
                    .Where(a => a.IsDeleted == false)
                    .AsQueryable();

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
                    .ToListAsync();

                var accountDtos = _mapper.Map<List<AccountDto>>(accounts);

                var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

                return new Pagination
                {
                    Account = accountDtos,
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
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountID == id && !a.IsDeleted);

                if (account == null)
                    return false;

                account.Status = (byte)Status;
                account.UpdatedAt = DateTime.Now;

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
        private async Task<string> GenerateTeacherCode()
        {
            var currentYear = DateTime.Now.Year;

            var lastTeacher = await _context.Teachers
                .Where(t => !t.IsDeleted && t.TeacherCode.StartsWith($"GV{currentYear}"))
                .OrderByDescending(t => t.TeacherCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastTeacher != null && !string.IsNullOrEmpty(lastTeacher.TeacherCode))
            {
                var numberPart = lastTeacher.TeacherCode.Substring(6); 
                if (int.TryParse(numberPart, out int number))
                {
                    nextNumber = number + 1;
                }
            }

            return $"GV{currentYear}{nextNumber:D3}"; 
        }

        private async Task<string> GenerateStudentCode()
        {
            var currentYear = DateTime.Now.Year;

            var lastStudent = await _context.Students
                .Where(s => !s.IsDeleted && s.StudentCode.StartsWith($"SV{currentYear}"))
                .OrderByDescending(s => s.StudentCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastStudent != null && !string.IsNullOrEmpty(lastStudent.StudentCode))
            {
                var numberPart = lastStudent.StudentCode.Substring(6); 
                if (int.TryParse(numberPart, out int number))
                {
                    nextNumber = number + 1;
                }
            }

            return $"SV{currentYear}{nextNumber:D3}"; 
        }
    }
}