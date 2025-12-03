using Student_management.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Department")]
public class Department
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

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    public ICollection<Teacher>? Teachers { get; set; }
    public ICollection<Class>? Classes { get; set; }
}