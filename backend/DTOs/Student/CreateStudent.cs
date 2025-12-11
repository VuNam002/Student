using Student_management.DTOs.Person;

namespace Student_management.DTOs.Student
{
    public class CreateStudent
    {
        public string? StudentCode { get; set; }
        public int ClassID { get; set; }
        public byte Status { get; set; } = 1;
        public DateTime? EnrollmentDate { get; set; }
        public CreatePersonDto ?Person { get; set; }
    }
}
