namespace Student_management.DTOs.Department
{
    public class PaginationDepartment
    {
        public List<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
