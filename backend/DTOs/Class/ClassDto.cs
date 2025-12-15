namespace Student_management.DTOs.Class
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassCode { get; set; }
        public int DepartmentId { get; set; }
        public int TeacherId { get; set; }
        public string AcademicYear { get; set; }
        public int Semester { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
