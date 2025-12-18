namespace Student_management.DTOs.Teacher
{
    public class TeacherDto
    {
        public int TeacherID { get; set; }
        public int PersonID { get; set; }
        public string? TeacherCode { get; set; }
        public int DepartmentID { get; set; }
        public string Position { get; set; }
        public string Degree { get; set; }
        public string Specialization { get; set; }
        public int AccountID { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? DepartmentName { get; set; } // Add this property to fix CS1061
    }
}
