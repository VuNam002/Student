using Student_management.DTOs.Teacher;

namespace Student_management.Services.Interfaces
{
    public interface ITeacherService 
    {
        Task<PaginationTeacher> GetTeacherPagination(TeacherSearch searchParams);
        Task<TeacherDto> CreateTeacher(CreateTeacher dto);
    }
}
