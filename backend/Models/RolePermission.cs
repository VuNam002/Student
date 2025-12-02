using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Role_Permission")]
    public class RolePermission
    {
        [Key]
        public int RolePermissionID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [Required]
        public int PermissionID { get; set; }

        [ForeignKey("RoleID")]
        public Role? Role { get; set; }

        [ForeignKey("PermissionID")]
        public Permission? Permission { get; set; }
    }
}