using Student_management.Models;
using Student_management.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Permission")]
public class Permission : BaseEntity
{
    [Key]
    public int PermissionID { get; set; }

    [Required]
    [StringLength(50)]
    public string? PermissionCode { get; set; } 

    [Required]
    [StringLength(100)]
    public string? PermissionName { get; set; }

    [StringLength(50)]
    public string? Module { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public ICollection<RolePermission>? RolePermissions { get; set; }
}