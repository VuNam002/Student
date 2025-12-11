using Student_management.Models;
using Student_management.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Role")]
public class Role : BaseEntity
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [StringLength(20)]
    public string? RoleCode { get; set; } 

    [Required]
    [StringLength(50)]
    public string? RoleName { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<Account>? Accounts { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}