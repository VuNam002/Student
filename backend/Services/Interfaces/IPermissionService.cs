namespace Student_management.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> AssignPermissionToRoleDto(int roleId, List<int> permissionIds);
    }
}
