using AutoMapper;
using Student_management.DTOs.Person;
using Student_management.DTOs.Teacher;
using Student_management.Models.Entities;

namespace Student_management.Mappings
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<Person, PersonDto>();
            CreateMap<Teacher, TeacherDto>();
        }
    }
}
