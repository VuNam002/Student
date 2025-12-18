using Student_management.DTOs.Department;

namespace Student_management.Services.Interfaces
{
    public interface IDeapartmentService
    {
        Task<PaginationDepartment> GetDepartmentPagination(DepartmentSearch search);
    }
}
