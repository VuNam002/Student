using Student_management.DTOs.Person;
using Student_management.Enum;

namespace Student_management.DTOs.Student
{
    public class StudentDto
    {
        public int StudentID { get; set; }
        public string? StudentCode { get; set; }
        public int PersonID { get; set; }
        public int ClassID { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public StudentStatus Status { get; set; } = StudentStatus.Active;
        public int? AccountID { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? ClassName { get; set; }
        public PersonDto? Person { get; set; }
    }
}
