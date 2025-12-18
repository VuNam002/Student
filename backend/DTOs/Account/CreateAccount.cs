namespace Student_management.DTOs.Account
{
    public class CreateAccount
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int RoleID { get; set; }
        public byte Status { get; set; } = 1;
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? IdentityCard { get; set; }
        public string? Avatar { get; set; }

        public int? DepartmentID { get; set; }
        public string? Position { get; set; }
        public string? Degree { get; set; }
        public string? Specialization { get; set; }

        public int? ClassID { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }
}