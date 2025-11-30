namespace Student_management.DTOs.Account
{
    public class AccountDetail
    {
        public int ID { get; set; }
        public string? Email { get; set; }
        public string? RoleID { get; set; }
        public string? Avatar { get; set; }
        public bool TrangThai { get; set; } = true;
        public string? TenHienThi { get; set; }
        public string? MatKhau { get; set; }
        public string? HoTen { get; set; }
        public string? SDT { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
