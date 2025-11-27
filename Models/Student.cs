using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("MaSV")]
        [StringLength(20)]
        public string? MaSV { get; set; }

        [Required]
        [Column("HoTen")]
        [StringLength(100)]
        public string? HoTen { get; set; }

        [Column("NgaySinh")]
        public DateTime? NgaySinh { get; set; } 


        [Column("LopID")]
        public int? LopID { get; set; }

        [Column("TaiKhoanID")]
        public int? TaiKhoanID { get; set; }

        [ForeignKey("LopID")]
        public Class? Class { get; set; }

        [ForeignKey("TaiKhoanID")]
        public Account? Account { get; set; }
    }
}
