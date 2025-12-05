using Student_management.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Student")]
public class Student
{
    [Key]
    public int StudentID { get; set; }

    [Required]
    public int PersonID { get; set; } 

    [Required]
    [StringLength(20)]
    public string? StudentCode { get; set; } 

    public int? ClassID { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EnrollmentDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? GraduationDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public int? AccountID { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Person? Person { get; set; } 
    public Class? Class { get; set; }
    public Account? Account { get; set; }
}