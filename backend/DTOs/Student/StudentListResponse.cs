namespace Student_management.DTOs.Student
{
    public class StudentListResponse
    {
        public List<StudentListItemDto> Data { get; set; }
        public int Total { get; set; }
    }
}