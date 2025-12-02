using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Teacher")]
    public class Teacher
    {
        [Key]
        public int TeacherID { get; set; }

        [Required]
        [StringLength(20)]
        public string ?MaGV { get; set; }

        [Required]
        [StringLength(100)]
        public string? HoTen { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public int? DepartmentID { get; set; }

        public int? AccountID { get; set; }

        // Navigation Properties
        [ForeignKey("DepartmentID")]
        public Department? Department { get; set; }

        [ForeignKey("AccountID")]
        public Account? Account { get; set; }
    }
}