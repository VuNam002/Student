using AutoMapper;
using Student_management.DTOs.Role;

namespace Student_management.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.PermissionIds, opt => opt.MapFrom(src =>
                    src.RolePermissions != null
                        ? src.RolePermissions.Select(rp => rp.PermissionID).ToList()
                        : new List<int>()));

            CreateMap<CreateRole, Role>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Accounts, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());
        }
    }
}
