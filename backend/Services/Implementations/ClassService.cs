using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Class;
using Student_management.Services.Interfaces;
using Student_management.Models;
using Student_management.Models.Entities;


namespace Student_management.Services.Implementations
{
    public class ClassService : IClassService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClassService> _logger;
        private readonly IMapper _mapper;

        public ClassService(AppDbContext context, ILogger<ClassService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<PaginationClass> GetClassPagination(ClassSearch searchParams)
        {
            try
            {
                var query = _context.Classes
                    .AsNoTracking()
                    .Include(c => c.Teacher)
                    .ThenInclude(t => t.Person)
                    .Include(c => c.Students)
                    .Include(c => c.Department)
                    .Where(c => !c.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(c => c.ClassCode!.ToLower().Contains(keyword) ||
                                             c.ClassName!.ToLower().Contains(keyword)); 
                }

                var totalCount = await query.CountAsync();

                var classes = await query
                    .Skip((searchParams.Page - 1) * searchParams.PageSize)
                    .Take(searchParams.PageSize)
                    .ToListAsync();

                var classDtos = _mapper.Map<List<ClassDto>>(classes);

                var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

                return new PaginationClass
                {
                    Classes = classDtos,
                    TotalCount = totalCount,
                    Page = searchParams.Page,
                    PageSize = searchParams.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting paginated classes.");
                throw;
            }
        }
        public async Task<ClassDto> GetClassById(int classId)
        {
            try
            {
                var classEntity = await _context.Classes
                    .AsNoTracking()
                    .Include(c => c.Teacher)
                    .ThenInclude(t => t.Person)
                    .Include(c => c.Department)
                    .Include(c => c.Students)
                    .FirstOrDefaultAsync(c => c.ClassID == classId && !c.IsDeleted);
                if (classEntity == null)
                {
                    throw new KeyNotFoundException("Khong thay lop hoc");
                }
                return _mapper.Map<ClassDto>(classEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting class by ID.");
                throw;
            }
        }
        public async Task<ClassDto> CreateClass(CreateClass dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var classExists = await _context.Classes
                    .AnyAsync(c => c.ClassCode == dto.ClassCode && !c.IsDeleted);
                var departmentExists = await _context.Departments
                    .AnyAsync(d => d.DepartmentID == dto.DepartmentId && !d.IsDeleted);
                if (!departmentExists)
                {
                    throw new KeyNotFoundException("Khong tim thay khoa");
                }
                if (dto.TeacherId.HasValue)
                {
                    var teacherExists = await _context.Teachers
                        .AnyAsync(t => t.TeacherID == dto.TeacherId.Value && !t.IsDeleted);
                    if (!teacherExists)
                    {
                        throw new KeyNotFoundException("Khong tim thay giao vien");
                    }
                }
                var newClass = _mapper.Map<Class>(dto);
                newClass.IsDeleted = false;
                newClass.CreatedAt = DateTime.UtcNow;
                newClass.UpdatedAt = DateTime.UtcNow;

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await GetClassById(newClass.ClassID);
                _logger.LogInformation("Created new class with ID {ClassID}", newClass.ClassID);
                return result;
            }
            catch (NotFoundException)
            {
                await transaction.RollbackAsync();
                throw;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while creating a new class.");
                throw;
            }
        }
        public async Task<ClassDto?> EditClass(int id, CreateClass dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var classEntity = await _context.Classes
                    .FirstOrDefaultAsync(c => c.ClassID == id && !c.IsDeleted);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found for editing.", id);
                    return null;
                }

                var departmentExists = await _context.Departments
                    .AnyAsync(d => d.DepartmentID == dto.DepartmentId && !d.IsDeleted);

                if (!departmentExists)
                {
                    throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");
                }

                if (dto.TeacherId.HasValue)
                {
                    var teacherExists = await _context.Teachers
                        .AnyAsync(t => t.TeacherID == dto.TeacherId.Value && !t.IsDeleted);

                    if (!teacherExists)
                    {
                        throw new KeyNotFoundException($"Teacher with ID {dto.TeacherId.Value} not found.");
                    }
                }

                if (classEntity.ClassCode != dto.ClassCode)
                {
                    var codeExists = await _context.Classes
                        .AnyAsync(c => c.ClassCode == dto.ClassCode && !c.IsDeleted && c.ClassID != id);

                    if (codeExists)
                    {
                        throw new InvalidOperationException($"Class code '{dto.ClassCode}' already exists.");
                    }
                }
                classEntity.ClassCode = dto.ClassCode;
                classEntity.ClassName = dto.ClassName;
                classEntity.DepartmentID = dto.DepartmentId;
                classEntity.TeacherID = dto.TeacherId;
                classEntity.Semester = dto.Semester;
                classEntity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await GetClassById(id);
                _logger.LogInformation("Updated class with ID {ClassID}", id);
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while editing class with ID {ClassId}", id);
                throw;
            }
        }
        public async Task<bool> DeleteClass(int id)
        {
            try
            {
                var classEntity = await _context.Classes
                    .FirstOrDefaultAsync(c => c.ClassID == id && !c.IsDeleted);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found for deletion.", id);
                    return false;
                }

                classEntity.IsDeleted = true;
                classEntity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Soft deleted class with ID {ClassID}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting class with ID {ClassId}", id);
                throw;
            }
        }

        public async Task<List<Student>> GetStudentsByClassId(int classId)
        {
            try
            {
                var students = await _context.Students
                    .AsNoTracking()
                    .Where(s => s.ClassID == classId && !s.IsDeleted)
                    .Include(s => s.Class)
                    .Include(s => s.Person)
                    .OrderBy(s => s.StudentID) 
                    .ToListAsync();

                return students;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting students for export by class ID {ClassId}", classId);
                throw;
            }
        }
    }
}
