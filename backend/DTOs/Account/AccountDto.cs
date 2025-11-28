namespace Student_management.DTOs.Account
{
    public class AccountDto
    {
        public int ID { get; set; }
        public string? Email { get; set; }
        public string? RoleID { get; set; }
        public string? Avatar {  get; set;  }
        public bool TrangThai { get; set; } = true;
        public string? TenHienThi { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}