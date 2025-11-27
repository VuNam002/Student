using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]

        [Column("MaRole")]
        public string? MaRole { get; set; }

        [Column("TenHienThi")]
        public string? TenHienThi { get; set; }

        public ICollection<Account>? Accounts { get; set; }
    }
}
