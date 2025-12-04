using Student_management.Data;
using Student_management.DTOs.Role;

namespace Student_management.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoleService> _logger;
        public RoleService(AppDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<RoleDto>> GetAll()
        {
            try
            {
                return await Task.FromResult(_context.Roles
                    .Where(r => r.IsDeleted == false)
                    .Select(r => new RoleDto
                    {
                        RoleID = r.RoleID,
                        RoleName = r.RoleName,
                        Description = r.Description,
                        IsDeleted = r.IsDeleted,
                        CreatedAt = r.CreatedAt
                    }).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all roles.");
                throw;
            }
        }
    }
}
