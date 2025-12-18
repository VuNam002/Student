using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Account;
using Student_management.DTOs.Class;
using Student_management.DTOs.Student;
using Student_management.Enum;

namespace Student_management.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PaginationStudent> GetStudentPagination(StudentSearch searchParams);
        Task<StudentDto> CreateStudent(CreateStudent dto);
        Task<StudentDto?> EditStudent(int id, CreateStudent dto);
        Task<bool> UpdateStudentStatus(int id, StudentStatus Status);
        Task<StudentDto?> Detail(int id);
        Task<StudentListResponse> GetStudentsByClass(int classId);
        Task<AddStudentsToClassResponse> AddStudentsToClass(int classId, List<int> studentIds);
        Task<RemoveStudentFromClassResponse> RemoveStudentFromClass(int classId, int studentId);
        Task<TransferStudentResponse> TransferStudentToClass(int studentId, int newClassId);

    }
}
