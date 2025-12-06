namespace Student_management.DTOs.Permission
{
    public class PermissionGroupDto
    {
        public string? Module { get; set; }
        public List<PermissionItemDto>? Permissions { get; set; }
    }
}
