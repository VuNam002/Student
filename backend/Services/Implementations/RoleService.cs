using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Role;
using Student_management.Services.Interfaces;

namespace Student_management.Services.Implementations
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
                return await _context.Roles
                    .Include(r => r.RolePermissions)
                    .Where(r => r.IsDeleted == false)
                    .Select(r => new RoleDto
                    {
                        RoleID = r.RoleID,
                        RoleName = r.RoleName,
                        RoleCode = r.RoleCode,
                        Description = r.Description,
                        IsDeleted = r.IsDeleted,
                        CreatedAt = r.CreatedAt,
                        PermissionIds = r.RolePermissions != null ? r.RolePermissions.Select(rp => rp.PermissionID).ToList() : new List<int>()
                    })
                    .ToListAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all roles.");
                throw;
            }
        }

        public async Task<RoleDto?> Detail(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Where(r => r.RoleID == id && r.IsDeleted == false)
                    .Select(r => new RoleDto
                    {
                        RoleID = r.RoleID,
                        RoleName = r.RoleName,
                        RoleCode = r.RoleCode,
                        Description = r.Description,
                        IsDeleted = r.IsDeleted,
                        CreatedAt = r.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role details for RoleID: {RoleID}", id);
                throw;
            }
        }

        public async Task<RoleDto> CreateRole(CreateRole dto)
        {
            try
            {
                var isExist = await _context.Roles
                    .AnyAsync(r => r.RoleCode == dto.RoleCode && !r.IsDeleted);

                if (isExist)
                {
                    throw new InvalidOperationException($"Role with code '{dto.RoleCode}' already exists.");
                }

                var newRole = new Role
                {
                    RoleCode = dto.RoleCode,
                    RoleName = dto.RoleName,
                    Description = dto.Description,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow 
                };
                await _context.Roles.AddAsync(newRole);
                await _context.SaveChangesAsync();
                return new RoleDto
                {
                    RoleID = newRole.RoleID,
                    RoleName = newRole.RoleName,
                    RoleCode = newRole.RoleCode,
                    Description = newRole.Description,
                    IsDeleted = newRole.IsDeleted,
                    CreatedAt = newRole.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating role with code: {RoleCode}", dto.RoleCode);
                throw;
            }
        }
        public async Task<RoleDto?> UpdateRole(int id, CreateRole dto)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleID == id && !r.IsDeleted);

                if (role == null)
                {
                    return null;
                }
                var isDuplicate = await _context.Roles
                    .AnyAsync(r => r.RoleCode == dto.RoleCode && r.RoleID != id && !r.IsDeleted);

                if (isDuplicate)
                {
                    throw new InvalidOperationException($"Role with code '{dto.RoleCode}' already exists.");
                }

                role.RoleCode = dto.RoleCode;
                role.RoleName = dto.RoleName;
                role.Description = dto.Description;

                await _context.SaveChangesAsync();

                return new RoleDto
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName,
                    RoleCode = role.RoleCode,
                    Description = role.Description,
                    IsDeleted = role.IsDeleted,
                    CreatedAt = role.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating role with ID: {RoleID}", id);
                throw;
            }
        }

        public async Task<bool> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleID == id && !r.IsDeleted);

                if (role == null)
                {
                    return false;
                }
                role.IsDeleted = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting role with ID: {RoleID}", id);
                throw;
            }
        }
    }
}