using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Class")]
    public class Class
    {
        [Key]
        public int ClassID { get; set; }

        [Required]
        [StringLength(100)]
        public string ?TenLop { get; set; }

        public int? DepartmentID { get; set; }

        [Column("TeacherID_GVCN")]
        public int? TeacherID_GVCN { get; set; }

        // Navigation Properties
        [ForeignKey("DepartmentID")]
        public Department? Department { get; set; }

        [ForeignKey("TeacherID_GVCN")]
        public Teacher? TeacherGVCN { get; set; }
    }
}