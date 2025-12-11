using Student_management.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models.Entities 
{
    [Table("Class")]
    public class Class : BaseEntity
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

        [ForeignKey(nameof(DepartmentID))]
        public Department? Department { get; set; }

        [ForeignKey(nameof(TeacherID))]
        public Teacher? Teacher { get; set; }

        public ICollection<Student>? Students { get; set; }
    }
}