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
                    TenHienThi = p.TenHienThi,
                    MaPermission = p.MaPermission,
                    Module = p.Module,
                    Description = p.description
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
                    TenHienThi = dto.TenHienThi,
                    Module = dto.Module,
                    description = dto.description
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                return new PermissionDto
                {
                    PermissionID = permission.PermissionID,
                    TenHienThi = permission.TenHienThi,
                    MaPermission = permission.MaPermission,
                    Module = permission.Module,
                    Description = permission.description
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
    }
}