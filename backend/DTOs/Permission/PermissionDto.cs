namespace Student_management.DTOs.Permission
{
    public class PermissionDto
    {
        public int PermissionID { get; set; }
        public string? PermissionCode { get; set; }
        public string? PermissionName { get; set; }
        public string? Module { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
