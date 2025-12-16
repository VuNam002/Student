namespace Student_management.DTOs.Class
{
    public class ClassSearch
    {
        public string? Keyword { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1; // Default value
        public int PageSize { get; set; } = 10; // Default value
    }
}
