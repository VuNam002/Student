using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        [Column("DepartmentID")]
        public int DepartmentID { get; set; }

        [Required]
        [Column("TenKhoa")]
        [StringLength(100)]
        public string? TenKhoa { get; set; }
        public ICollection<Teacher>? Teachers { get; set; }

        public ICollection<Class>? Classes { get; set; }
    }
}