namespace Student_management.DTOs.Account
{
    public class AccountDto
    {
        public int ID { get; set; }
        public string? TenDangNhap { get; set; }
        public string? VaiTroID { get; set; }
        public string? Avatar {  get; set;  }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}