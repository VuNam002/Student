namespace Student_management.DTOs.Class
{
    public class PaginationClass
    {
        public List<ClassDto> Classes { get; set; } = new List<ClassDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
