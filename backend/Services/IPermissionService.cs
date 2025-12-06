namespace Student_management.Services
{
    public interface IPermissionService
    {
        Task<bool> AssignPermissionToRoleDto(int roleId, List<int> permissionIds);

    }
}
