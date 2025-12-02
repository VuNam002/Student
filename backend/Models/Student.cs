using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [StringLength(20)]
        public string? MaSV { get; set; }

        [Required]
        [StringLength(100)]
        public string ?HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public int? ClassID { get; set; }

        public int? AccountID { get; set; }

        // Navigation Properties
        [ForeignKey("ClassID")]
        public Class? Class { get; set; }

        [ForeignKey("AccountID")]
        public Account? Account { get; set; }
    }
}