using Student_management.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Class")]
public class Class
{
    [Key]
    public int ClassID { get; set; }

    [Required]
    [StringLength(100)]
    public string? ClassName { get; set; }

    [Required]
    [StringLength(20)]
    public string? ClassCode { get; set; } 

    public int? DepartmentID { get; set; }

    public int? TeacherID { get; set; } 

    [StringLength(20)]
    public string? AcademicYear { get; set; }

    public int? Semester { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Department? Department { get; set; }
    public Teacher? Teacher { get; set; }
    public ICollection<Student>? Students { get; set; }
}