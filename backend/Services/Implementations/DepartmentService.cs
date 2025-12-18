using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Department;
using Student_management.Services.Interfaces;

namespace Student_management.Services.Implementations
{
    public class DepartmentService : IDeapartmentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DepartmentService> _logger;
        private readonly IMapper _mapper;

        public DepartmentService(AppDbContext context, ILogger<DepartmentService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginationDepartment> GetDepartmentPagination(DepartmentSearch search)
        {
            try
            {
                var query = _context.Departments
                    .AsNoTracking()
                    .Include(d => d.Teachers)
                    .Include(d => d.Classes)
                    .Where(d => !d.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search.Keyword))
                {
                    var keyword = search.Keyword.Trim().ToLower();
                    query = query.Where(d => d.DepartmentName.ToLower().Contains(keyword) ||
                                            (d.Description != null && d.Description.ToLower().Contains(keyword)));
                }

                var totalCount = await query.CountAsync();
                var departments = await query
                    .Skip((search.Page - 1) * search.PageSize)
                    .Take(search.PageSize)
                    .ToListAsync();

                var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);
                var totalPages = (int)Math.Ceiling(totalCount / (double)search.PageSize);

                return new PaginationDepartment
                {
                    Departments = departmentDtos,
                    TotalCount = totalCount,
                    Page = search.Page,
                    PageSize = search.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting paginated departments.");
                throw;
            }
        }
    }
}