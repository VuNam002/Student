using Student_management.DTOs.Account;

namespace Student_management.DTOs.Student
{
    public class PaginationStudent
    {
        public List<StudentDto> Student { get; set; } = new List<StudentDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
