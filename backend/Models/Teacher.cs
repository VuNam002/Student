using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Teacher")]
    public class Teacher
    {
        [Key]
        [Column("TeacherID")]
        public int TeacherID { get; set; }

        [Required]
        [Column("MaGV")]
        public string? MaGV { get; set; }

        [Column("HoTen")]
        public string? HoTen { get; set; }

        [Column("Email")]
        public string? Email { get; set; }

        [Column("KhoaID")]
        public int? KhoaID { get; set; }

        [Column("TaiKhoanID")]
        public int? TaiKhoanID { get; set; }
        [ForeignKey("KhoaID")]
        public Department? Department { get; set; }

        [ForeignKey("TaiKhoanID")]
        public Account? Account { get; set; }

        public ICollection<Class>? Classes { get; set; }
    }
}
