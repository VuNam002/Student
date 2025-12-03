using Student_management.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Role")]
public class Role
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

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public ICollection<Account>? Accounts { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}