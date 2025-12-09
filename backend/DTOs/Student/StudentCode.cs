namespace Student_management.DTOs.Student
{
    public class StudentCode
    {
        public string? Keyword { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1; 
        public int PageSize { get; set; } = 10;
    }
}
