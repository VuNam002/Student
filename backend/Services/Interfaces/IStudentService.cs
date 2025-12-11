using Student_management.DTOs.Account;
using Student_management.DTOs.Student;
using Student_management.Enum;

namespace Student_management.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PaginationStudent> GetStudentPagination(StudentSearch searchParams);
        Task<StudentDto?> EditStudent(int id, CreateStudent dto);
        Task<StudentDto> CreateStudent(CreateStudent dto);
        Task<bool> UpdateStudentStatus(int id, StudentStatus Status);
        Task<StudentDto?> Detail(int id);
    }
}
