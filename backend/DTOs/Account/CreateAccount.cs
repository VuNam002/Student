namespace Student_management.DTOs.Account
{
    public class CreateAccount
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int RoleID { get; set; }
        public string? Avatar { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public byte Status { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
