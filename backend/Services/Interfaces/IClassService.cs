using Student_management.DTOs.Class;

namespace Student_management.Services.Interfaces
{
    public interface IClassService
    {
        Task<PaginationClass> GetClassPagination(ClassSearch searchParams);
    }
}
