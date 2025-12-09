using Student_management.DTOs.Account;
using Student_management.DTOs.Student;

namespace Student_management.Services
{
    public interface IStudentService
    {
        Task<PaginationStudent> GetStudentPagination(StudentSearch searchParams);
    }
}
