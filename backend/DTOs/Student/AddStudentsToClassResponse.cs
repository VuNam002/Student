namespace Student_management.DTOs.Student
{
    public class AddStudentsToClassResponse
    {
        public string Message { get; set; }
        public int AddedCount { get; set; }
        public List<string> FailedStudents { get; set; } = new List<string>();
    }
}
