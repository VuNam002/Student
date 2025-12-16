using AutoMapper;
using Student_management.DTOs.Permission;

namespace Student_management.Mappings
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<Permission, DTOs.Permission.PermissionDto>();
            CreateMap<Permission, PermissionItemDto>();

            CreateMap<CreatePermission, Permission>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PermissionID, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());
        }
    }
}
