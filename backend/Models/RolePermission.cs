using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RolePermission")]
public class RolePermission
{
    [Key]
    public int RolePermissionID { get; set; }

    [Required]
    public int RoleID { get; set; }

    [Required]
    public int PermissionID { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
