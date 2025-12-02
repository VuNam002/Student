using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(20)]
        public string? MaRole { get; set; }

        [Required]
        [StringLength(50)]
        public string? TenHienThi { get; set; }
    }
}