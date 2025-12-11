using Student_management.Models.Base;
using Student_management.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Department")]
public class Department : BaseEntity
{
    [Key]
    public int DepartmentID { get; set; }

    [Required]
    [StringLength(20)]
    public string? DepartmentCode { get; set; }

    [Required]
    [StringLength(100)]
    public string? DepartmentName { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public ICollection<Teacher>? Teachers { get; set; }
    public ICollection<Class>? Classes { get; set; }
}