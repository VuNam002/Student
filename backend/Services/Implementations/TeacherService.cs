using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Student;
using Student_management.DTOs.Teacher;
using Student_management.Models.Entities;
using Student_management.Services.Interfaces;

namespace Student_management.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TeacherService> _logger;
        private readonly IMapper _mapper;

        public TeacherService(AppDbContext context, ILogger<TeacherService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginationTeacher> GetTeacherPagination(TeacherSearch searchParams)
        {
            try
            {
                var query = _context.Teachers
                    .AsNoTracking()
                    .Include(s => s.Person)
                    .Include(s => s.Classes)
                    .Include(s => s.Account)
                    .Include(s =>s.Department)
                    .Where(s => !s.IsDeleted)
                    .AsQueryable();
                if(!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(s => s.TeacherCode!.ToLower().Contains(keyword));
                }
                var totalCount = await query.CountAsync();
                var teachers = await query
                    .Skip((searchParams.Page - 1) * searchParams.PageSize)
                    .Take(searchParams.PageSize)
                    .ToListAsync();
                var teacherDtos = _mapper.Map<List<TeacherDto>>(teachers);
                var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

                return new PaginationTeacher
                {
                    Teachers = teacherDtos,
                    TotalCount = totalCount,
                    Page = searchParams.Page,
                    PageSize = searchParams.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated teachers.");
                throw;
            }
        }
        public async Task<TeacherDto> CreateTeacher(CreateTeacher dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var teacherExists = await _context.Teachers
                    .Where(t => !t.IsDeleted && t.TeacherCode == dto.TeacherCode)
                    .AnyAsync();
                if (!teacherExists)
                {
                    throw new KeyNotFoundException("Teacher with the same code already exists.");
                }
                var person = _mapper.Map<Person>(dto.PersonID);
                var teacher = _mapper.Map<Teacher>(dto);
                teacher.Person = person;

                _context.Teachers.Add(teacher);
                _context.Persons.Add(person);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var createdTeacher = await _context.Teachers
                    .AsNoTracking()
                    .Include(t => t.Person)
                    .Include(t => t.Department)
                    .Include(t => t.Account)
                    .FirstOrDefaultAsync(t => t.TeacherID == teacher.TeacherID);
                return _mapper.Map<TeacherDto>(createdTeacher);
            } catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while creating a new teacher.");
                throw;
            }
        }
    }
}
