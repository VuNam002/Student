using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Account;
using Student_management.DTOs.Person;
using Student_management.DTOs.Student;
using Student_management.Enum;
using Student_management.Models;
using Student_management.Models.Entities;
using Student_management.Services.Interfaces;
using AutoMapper;

namespace Student_management.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;

        public StudentService(AppDbContext context, ILogger<StudentService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
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
                    .ToListAsync();
                // AutoMapper
                var studentDtos = _mapper.Map<List<StudentDto>>(students);

                var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

                return new PaginationStudent
                {
                    Student = studentDtos,
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
                // AutoMapper
                var person = _mapper.Map<Person>(dto.Person);
                var student = _mapper.Map<Student>(dto);
                student.Person = person;

                _context.Persons.Add(person);
                _context.Students.Add(student);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var createdStudent = await _context.Students
                    .Include(s => s.Person)
                    .Include(s => s.Class)
                    .FirstAsync(s => s.StudentID == student.StudentID);

                return _mapper.Map<StudentDto>(createdStudent);
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
                // Update Person fields
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
                var updatedStudent = await _context.Students
                    .Include(s => s.Person)
                    .Include(s => s.Class)
                    .Include(s => s.Account)
                    .FirstAsync(s => s.StudentID == id);

                return _mapper.Map<StudentDto>(updatedStudent);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while editing student with ID {StudentId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateStudentStatus(int id, StudentStatus Status)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return false;
                }

                student.Status = (StudentStatus)(byte)Status;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi cap nhat trang thai");
                throw;
            }
        }
        public async Task<StudentDto?> Detail(int id)
        {
            try
            {
                var student = await _context.Students
                    .AsNoTracking()
                    .Include(s => s.Person)
                    .Include(s => s.Class)
                    .Include(s => s.Account)
                    .Where(s => s.StudentID == id && !s.IsDeleted)
                    .FirstOrDefaultAsync();
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay chi tiet sinh vien");
                throw;
            }
        }
    }
}