using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Permission;
using Student_management.Models;

namespace Student_management.Services
{
    public class PermissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(AppDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PermissionDto>> GetAllPermissions(string? module = null)
        {
            try
            {
                var query = _context.Permissions.AsQueryable();

                if (!string.IsNullOrWhiteSpace(module))
                {
                    query = query.Where(p => p.Module == module);
                }

                var result = await query.Select(p => new PermissionDto
                {
                    PermissionID = p.PermissionID,
                    PermissionName = p.PermissionName,
                    PermissionCode = p.PermissionCode,
                    Module = p.Module,
                    Description = p.Description,
                    IsDeleted = p.IsDeleted,
                    CreatedAt = p.CreatedAt,
                }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving permissions.");
                throw;
            }
        }

        public async Task<PermissionDto> CreatePermissions(CreatePermission dto)
        {
            try
            {
                var permission = new Permission
                {
                    PermissionName = dto.PermissionName,
                    Module = dto.Module,
                    Description = dto.Description
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                return new PermissionDto
                {
                    PermissionID = permission.PermissionID,
                    PermissionName = permission.PermissionName,
                    PermissionCode = permission.PermissionCode,
                    Module = permission.Module,
                    Description = permission.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating permission.");
                throw;
            }
        }

        public async Task<bool> DeletePermission(int id)
        {
            try
            {
                var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionID == id);

                if (permission == null)
                {
                    _logger.LogWarning("Permission not found for deletion with ID: {id}", id);
                    return false;
                }

                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Permission deleted successfully: {id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting permission.");
                throw;
            }
        }
        public async Task<bool> AssignPermissionToRoleDto(int roleId, List<int> permissionIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra role tồn tại
                var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == roleId);
                if (!roleExists)
                {
                    _logger.LogWarning("Role not found with ID: {roleId}", roleId);
                    return false;
                }

                // Xóa permissions cũ
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleID == roleId)
                    .ToListAsync();

                if (existingPermissions.Any())
                {
                    _context.RolePermissions.RemoveRange(existingPermissions);
                }

                // Validate và lấy permissions hợp lệ
                var validPermissionIds = await _context.Permissions
                    .Where(p => permissionIds.Contains(p.PermissionID))
                    .Select(p => p.PermissionID)
                    .ToListAsync();

                if (!validPermissionIds.Any())
                {
                    _logger.LogWarning("No valid permissions found for role {roleId}", roleId);
                    await transaction.RollbackAsync();
                    return false;
                }

                // Thêm permissions mới
                var newRolePermissions = validPermissionIds.Select(permissionId => new RolePermission
                {
                    RoleID = roleId,
                    PermissionID = permissionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                await _context.RolePermissions.AddRangeAsync(newRolePermissions);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully assigned {count} permissions to role {roleId}",
                    newRolePermissions.Count, roleId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error assigning permissions to role {roleId}", roleId);
                throw;
            }
        }
    }
}