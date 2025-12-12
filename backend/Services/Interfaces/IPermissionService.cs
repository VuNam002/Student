using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Permission;
using Student_management.Models;

namespace Student_management.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PermissionGroupDto>> GetAllPermissionsGrouped(string? module = null);

        Task<PermissionDto> CreatePermissions(CreatePermission dto);

        Task<bool> DeletePermission(int id);
        Task<bool> AssignPermissionToRoleDto(int roleId, List<int> permissionIds);
    }
}