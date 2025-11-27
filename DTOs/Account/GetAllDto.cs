using Student_management.Models;

namespace Student_management.DTOs.Account
{
    public class GetAllDto
    {
        public string? TenDangNhap {  get; set; }
        public string? Avatar { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int VaiTroID { get; set; }
        public Role? Role { get; set; }
    }
}
