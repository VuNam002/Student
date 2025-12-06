namespace Student_management.DTOs.Role
{
    public class RoleDto
    {
        public int RoleID { get; set; }
        public string? RoleCode { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<int>? PermissionIds { get; set; }
    }
}
