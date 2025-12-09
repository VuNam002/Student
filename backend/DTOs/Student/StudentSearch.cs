namespace Student_management.DTOs.Student
{
    public class StudentSearch
    {
        public string? Keyword { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1; // Default value
        public int PageSize { get; set; } = 10; // Default value
    }
}
