namespace Student_management.DTOs.Teacher
{
    public class PaginationTeacher
    {
        public List<TeacherDto> Teachers { get; set; } = new List<TeacherDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
