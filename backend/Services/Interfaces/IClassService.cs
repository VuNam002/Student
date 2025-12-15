using Student_management.DTOs.Class;
using Student_management.Enum;

namespace Student_management.Services.Interfaces
{
    public interface IClassService
    {
        Task<PaginationClass> GetClassPagination(ClassSearch searchParams);
        Task<ClassDto> GetClassById(int classId);
        Task<ClassDto> CreateClass(CreateClass dto);
        Task<ClassDto?> EditClass(int id, CreateClass dto);
        Task<bool> DeleteClass(int id);
    }
}
