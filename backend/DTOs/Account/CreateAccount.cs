namespace Student_management.DTOs.Account
{
    public class CreateAccount
    {
        public string? Email { get; set; }
        public string? MatKhau { get; set; }
        public int RoleID { get; set; }
        public string? Avatar { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
