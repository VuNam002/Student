namespace Student_management.DTOs.Account
{
    public class AccountDto
    {
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
    }
}