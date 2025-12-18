using AutoMapper;
using Student_management.DTOs.Department;
using Student_management.Models.Entities;

namespace Student_management.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Teacher, TeacherProfile>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<Class, ClassProfile>();
        }
    }
}
