namespace Student_management.DTOs.Account
{
    public class AccountDetail
    {
        public int AccountID { get; set; }
        public string? Email { get; set; }
        public string? RoleID { get; set; }
        public string? Avatar { get; set; }
        public bool Status { get; set; } = true;
        public string? RoleName { get; set; }
        public string? Password { get; set; }
        public string? HoTen { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
