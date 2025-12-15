using AutoMapper;

namespace Student_management.Mappings
{
    public class ClassProfile : Profile
    {
        public ClassProfile ()
        {
            CreateMap<DTOs.Class.ClassDto, Models.Entities.Class>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
