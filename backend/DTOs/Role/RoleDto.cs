namespace Student_management.DTOs.Role
{
    public class RoleDto
    {
        public int RoleID { get; set; }
        public int RoleCode { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
