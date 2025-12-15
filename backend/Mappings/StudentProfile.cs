using AutoMapper;
using Student_management.DTOs.Permission;
using Student_management.DTOs.Person;
using Student_management.DTOs.Student;
using Student_management.Enum;
using Student_management.Models.Entities;

namespace Student_management.Mappings
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src =>
                    src.Class != null ? src.Class.ClassName : null))
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person));

            CreateMap<Person, PersonDto>();

            CreateMap<CreateStudent, Student>()
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src =>
                src.EnrollmentDate ?? DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StudentStatus.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Person, opt => opt.Ignore())
                .ForMember(dest => dest.StudentID, opt => opt.Ignore())
                .ForMember(dest => dest.GraduationDate, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore());

            CreateMap<CreatePersonDto, Person>()
                .ForMember(dest => dest.PersonType, opt => opt.MapFrom(src => "STUDENT"))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PersonID, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());
        }
    }
}
