using Student_management.DTOs.Role;

namespace Student_management.Services
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAll();
    }
}