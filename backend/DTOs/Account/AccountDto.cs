namespace Student_management.DTOs.Account
{
    public class AccountDto
    {
        // Account info
        public int ID { get; set; }
        public string? Email { get; set; }
        public string? RoleID { get; set; }
        public string? Avatar { get; set; }
        public byte Status { get; set; }
        public string? RoleName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Person info
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? IdentityCard { get; set; }

        // Teacher info
        public int? DepartmentID { get; set; }
        public string? Position { get; set; }
        public string? Degree { get; set; }
        public string? Specialization { get; set; }

        // Student info
        public int? ClassID { get; set; }
         // Alias for ClassID
        public DateTime? EnrollmentDate { get; set; }
    }
}