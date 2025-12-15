namespace Student_management.DTOs.Class
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string AcademicYear { get; set; }
        public int Semester { get; set; }
        public int TotalStudents { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
