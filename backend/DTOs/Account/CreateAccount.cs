namespace Student_management.DTOs.Account
{
    public class CreateAccount
    {
        public string? Email { get; set; }
        public string? MatKhau { get; set; }
        public int RoleID { get; set; }
        public string? Avatar { get; set; }
        public string? HoTen { get; set; }
        public string? SDT { get; set; }
        public byte TrangThai { get; set; } = 1;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
