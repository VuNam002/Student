using Student_management.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RolePermission")]
public class RolePermission : BaseEntity
{
    [Key]
    public int RolePermissionID { get; set; }

    [Required]
    public int RoleID { get; set; }

    [Required]
    public int PermissionID { get; set; }

    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
