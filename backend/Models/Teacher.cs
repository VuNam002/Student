using Student_management.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Teacher")]
public class Teacher
{
    [Key]
    public int TeacherID { get; set; }

    [Required]
    public int PersonID { get; set; } 

    [Required]
    [StringLength(20)]
    public string? TeacherCode { get; set; } 

    public int? DepartmentID { get; set; }

    [StringLength(50)]
    public string? Position { get; set; }

    [StringLength(50)]
    public string? Degree { get; set; }

    [StringLength(100)]
    public string? Specialization { get; set; }

    public int? AccountID { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ForeignKey("PersonID")]
    public Person? Person { get; set; } 

    [ForeignKey("DepartmentID")]
    public Department? Department { get; set; }

    [ForeignKey("AccountID")]
    public Account? Account { get; set; }

    // Reverse Navigation
    public ICollection<Class>? Classes { get; set; } 
}