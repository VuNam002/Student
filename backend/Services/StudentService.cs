using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Account;
using Student_management.DTOs.Person;
using Student_management.DTOs.Student;
using Student_management.Enum;
using Student_management.Models;

namespace Student_management.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;
        public StudentService(AppDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginationStudent> GetStudentPagination(StudentSearch searchParams)
        {
            try
            {
                var query = _context.Students
                    .AsNoTracking()
                    .Include(s => s.Person)
                    .Include(s => s.Class)
                    .Include(s => s.Account)
                    .Where(s => !s.IsDeleted)
                    .AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(s => s.StudentCode!.ToLower().Contains(keyword));
                }
                if (searchParams.Status.HasValue)
                {
                    query = query.Where(s => (int)s.Status == searchParams.Status.Value);
                }
                var totalCount = await query.CountAsync();

                var students = await query
                    .Skip((searchParams.Page - 1) * searchParams.PageSize)
                    .Take(searchParams.PageSize)
                    .Select(s => new StudentDto
                    {
                        StudentID = s.StudentID,
                        PersonID = s.PersonID, 
                        StudentCode = s.StudentCode,
                        ClassID = s.ClassID, 
                        ClassName = s.Class != null ? s.Class.ClassName : null,
                        EnrollmentDate = s.EnrollmentDate,
                        GraduationDate = s.GraduationDate,
                        Status = s.Status,
                        AccountID = s.AccountID, 
                        Person = s.Person == null ? null : new PersonDto
                        {
                            PersonID = s.Person.PersonID,
                            FullName = s.Person.FullName,
                            DateOfBirth = s.Person.DateOfBirth,
                            Gender = s.Person.Gender,
                            Email = s.Person.Email,
                            PhoneNumber = s.Person.PhoneNumber,
                            Address = s.Person.Address
                        }
                    })
                    .ToListAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);
                return new PaginationStudent
                {
                    Student = students,
                    TotalCount = totalCount,
                    Page = searchParams.Page,
                    PageSize = searchParams.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting student pagination.");
                throw;
            }
        }

        public async Task<StudentDto> CreateStudent(CreateStudent dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var classExists = await _context.Classes
                    .Where(c => c.ClassID == dto.ClassID && !c.IsDeleted)
                    .AnyAsync();

                if (!classExists)
                {
                    throw new KeyNotFoundException($"Class with ID {dto.ClassID} not found.");
                }
                var codeExists = await _context.Students
                    .AnyAsync(s => s.StudentCode == dto.StudentCode && !s.IsDeleted);

                if (codeExists)
                {
                    throw new InvalidOperationException($"Student code {dto.StudentCode} already exists.");
                }
                var person = new Person
                {
                    FullName = dto.Person.FullName,
                    DateOfBirth = dto.Person.DateOfBirth,
                    Gender = dto.Person.Gender,
                    Email = dto.Person.Email,
                    PhoneNumber = dto.Person.PhoneNumber,
                    Address = dto.Person.Address,
                    PersonType = "STUDENT",
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var student = new Student
                {
                    Person = person, 
                    StudentCode = dto.StudentCode,
                    ClassID = dto.ClassID,
                    EnrollmentDate = dto.EnrollmentDate ?? DateTime.UtcNow,
                    Status = StudentStatus.Active,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Persons.Add(person);
                _context.Students.Add(student);

                await _context.SaveChangesAsync(); 

                await transaction.CommitAsync();

                var className = await _context.Classes
                    .Where(c => c.ClassID == student.ClassID)
                    .Select(c => c.ClassName)
                    .FirstOrDefaultAsync();

                return new StudentDto
                {
                    StudentID = student.StudentID,
                    StudentCode = student.StudentCode,
                    ClassID = student.ClassID,
                    ClassName = className,
                    EnrollmentDate = student.EnrollmentDate,
                    Status = student.Status,
                    PersonID = person.PersonID,
                    Person = new PersonDto
                    {
                        PersonID = person.PersonID,
                        FullName = person.FullName,
                        Email = person.Email,
                        PhoneNumber = person.PhoneNumber,
                        Address = person.Address,
                        Gender = person.Gender,
                        DateOfBirth = person.DateOfBirth,
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error while creating student");
                throw;
            }
        }

        public async Task<StudentDto?> EditStudent(int id, CreateStudent dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Students
                    .Include(s => s.Person)
                    .FirstOrDefaultAsync(s => s.StudentID == id && s.IsDeleted == false);

                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found for editing.", id);
                    return null;
                }
                var classExists = await _context.Classes.AnyAsync(c => c.ClassID == dto.ClassID && !c.IsDeleted);
                if (!classExists)
                {
                    throw new KeyNotFoundException($"Class with ID {dto.ClassID} not found.");
                }
                if (student.StudentCode != dto.StudentCode)
                {
                    var codeExists = await _context.Students.AnyAsync(s => s.StudentCode == dto.StudentCode && !s.IsDeleted);
                    if (codeExists)
                    {
                        throw new InvalidOperationException($"Student code {dto.StudentCode} already exists.");
                    }
                }
                student.StudentCode = dto.StudentCode;
                student.ClassID = dto.ClassID;
                student.UpdatedAt = DateTime.UtcNow;
                if (dto.EnrollmentDate.HasValue)
                {
                    student.EnrollmentDate = dto.EnrollmentDate.Value;
                }
                if (student.Person != null && dto.Person != null)
                {
                    student.Person.FullName = dto.Person.FullName;
                    student.Person.PhoneNumber = dto.Person.PhoneNumber;
                    student.Person.Address = dto.Person.Address;
                    student.Person.Email = dto.Person.Email;
                    student.Person.Gender = dto.Person.Gender;
                    student.Person.DateOfBirth = dto.Person.DateOfBirth;
                    student.Person.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                var studentDto = await GetStudentPagination(new StudentSearch { Keyword = student.StudentCode, Page = 1, PageSize = 1 });
                return studentDto.Student.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while editing student with ID {StudentId}", id);
                throw;
            }
        }
    }
}
