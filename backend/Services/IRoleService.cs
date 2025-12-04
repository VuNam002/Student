using Student_management.DTOs.Role;

namespace Student_management.Services
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAll();
        Task<RoleDto?> Detail(int id);
        Task<RoleDto> CreateRole(CreateRole dto);
        Task<RoleDto?> UpdateRole(int id, CreateRole dto);
        Task<bool> DeleteRole(int id);
    }
}