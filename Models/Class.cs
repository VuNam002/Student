using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Class")]
    public class Class
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("TenLop")]
        [StringLength(100)]
        public string? TenLop { get; set; }
        [Column("KhoaID")]
        public int? KhoaID { get; set; }


        [Column("GVCN_ID")]
        public int? GVCN_ID { get; set; }

        [ForeignKey("KhoaID")]
        public Department? Department { get; set; }

        [ForeignKey("GVCN_ID")]
        public Teacher? TeacherGVCN { get; set; }

        public ICollection<Student>? Students { get; set; }
    }
}