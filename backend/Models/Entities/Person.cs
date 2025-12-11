using Student_management.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Student_management.Models.Entities;

namespace Student_management.Models.Entities
{
    [Table("Person")]
    public class Person : BaseEntity
    {
        [Key]
        public int PersonID { get; set; }

        [Required]
        [StringLength(20)]
        public string PersonType { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? IdentityCard { get; set; }

        public Teacher? Teacher { get; set; }
        public Student? Student { get; set; }
    }
}
   