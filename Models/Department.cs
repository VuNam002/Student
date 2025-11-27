using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("TenKhoa")]
        [StringLength(100)]
        public string? TenKhoa { get; set; }
        public ICollection<Teacher>? Teachers { get; set; }

        public ICollection<Class>? Classes { get; set; }
    }
}