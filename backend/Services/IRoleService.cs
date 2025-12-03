using Student_management.DTOs.Role;

namespace Student_management.Services
{
    public interface IRoleService
    {
        Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
    }
}