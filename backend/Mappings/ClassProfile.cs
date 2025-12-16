using AutoMapper;

namespace Student_management.Mappings
{
    public class ClassProfile : Profile
    {
        public ClassProfile()
        {
            // Class Entity -> ClassDto 
            CreateMap<Models.Entities.Class, DTOs.Class.ClassDto>()
                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null))
                .ForMember(dest => dest.TeacherName,
                    opt => opt.MapFrom(src => src.Teacher != null && src.Teacher.Person != null
                        ? src.Teacher.Person.FullName
                        : null))
                .ForMember(dest => dest.TotalStudents,
                    opt => opt.MapFrom(src => src.Students != null
                        ? src.Students.Count(s => !s.IsDeleted)
                        : 0))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // ClassDto -> Class Entity 
            CreateMap<DTOs.Class.ClassDto, Models.Entities.Class>()
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore());

            // CreateClassDto -> Class Entity
            CreateMap<DTOs.Class.CreateClass, Models.Entities.Class>()
                .ForMember(dest => dest.ClassID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore());

            // UpdateClassDto -> Class Entity
            CreateMap<DTOs.Class.UpdateClassDto, Models.Entities.Class>()
                .ForMember(dest => dest.ClassID, opt => opt.Ignore())
                .ForMember(dest => dest.ClassCode, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}