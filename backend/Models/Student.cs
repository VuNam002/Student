using Student_management.Models;
using Student_management.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Student_management.Models
{
    [Table("Student")]
    [Index(nameof(StudentCode), IsUnique = true)]
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        public int PersonID { get; set; }

        [Required]
        [StringLength(10)]
        public string StudentCode { get; set; } = string.Empty;

        public int ClassID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EnrollmentDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? GraduationDate { get; set; }

        public StudentStatus Status { get; set; } = StudentStatus.Active;

        public int? AccountID { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(PersonID))]
        public Person? Person { get; set; }

        [ForeignKey(nameof(ClassID))]
        public Class? Class { get; set; }

        [ForeignKey(nameof(AccountID))]
        public Account? Account { get; set; }
    }
}