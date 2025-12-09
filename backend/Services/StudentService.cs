using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Account;
using Student_management.DTOs.Student;
using Student_management.Enum;

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
                var query = _context.Students.AsNoTracking().Include(s => s.Account).Where(s => s.IsDeleted == false).AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(s => s.StudentCode!.ToLower().Contains(keyword));
                }
                if (searchParams.Status.HasValue)
                {
                    query = query.Where(s => s.Status == (StudentStatus)searchParams.Status.Value);
                }
                var totalCount = await query.CountAsync();
                var students = await query
                    .Skip((searchParams.Page - 1) * searchParams.PageSize)
                    .Take(searchParams.PageSize)
                    .Include(s => s.Person)
                    .Include(s => s.Class)
                    .Include(s => s.Account)
                    .Select(s => new StudentDto
                    {
                        StudentID = s.StudentID,
                        StudentCode = s.StudentCode,
                        FullName = s.Person.FullName,
                        DateOfBirth = s.Person.DateOfBirth,
                        Gender = s.Person.Gender,
                        PhoneNumber = s.Person.PhoneNumber,
                        Address = s.Person.Address,
                        IdentityCard = s.Person.IdentityCard,
                        Email = s.Account.Email,
                        Avatar = s.Account.Avatar,
                        CreatedAt = s.CreatedAt,
                    }).ToListAsync();
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
    }
}
