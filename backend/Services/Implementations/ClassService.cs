using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Class;
using Student_management.Services.Interfaces;

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
                    .Include(c => c.Students)
                    .Include(c => c.Department)
                    .Where(c => !c.IsDeleted) 
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchParams.Keyword))
                {
                    var keyword = searchParams.Keyword.Trim().ToLower();
                    query = query.Where(c => c.ClassCode!.ToLower().Contains(keyword) ||
                                             c.ClassName!.ToLower().Contains(keyword)); // Thêm search theo ClassName
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
    }
}
