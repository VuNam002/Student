using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Account")]
    public class Account
    {
        [Key]
        [Column("AccountID")]
        public int AccountID { get; set; }

        [Required]
        [Column("Email")]
        [StringLength(50)]
        public string? Email { get; set; }

        [Required]
        [Column("MatKhau")]
        [StringLength(255)]
        public string? MatKhau { get; set; }

        [Column("Avatar")]
        [StringLength(255)]
        public string? Avatar { get; set; }

        [Column("TrangThai")]
        public bool TrangThai { get; set; } = true; 

        [Column("NgayTao")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("RoleID")]
        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public Role? Role { get; set; }

        public Teacher? Teacher { get; set; }

        public Student? Student { get; set; }
    }
}