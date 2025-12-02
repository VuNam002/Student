using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Permission")]
    public class Permission
    {
        [Key]
        public int PermissionID { get; set; }

        [Required]
        [StringLength(50)]
        public string? MaPermission { get; set; }

        [Required]
        [StringLength(100)]
        public string? TenHienThi { get; set; }

        [StringLength(50)]
        public string? Module { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }
    }
}