namespace Student_management.DTOs.Role
{
    public class RoleDto
    {
        public int ID { get; set; }
        public int MaRole { get; set; }
        public string? TenHienThi { get; set; }
        public string? description { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
