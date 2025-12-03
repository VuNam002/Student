
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_management.Models
{
    [Table("Person")]
    public class Person
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

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Properties (1-1 relationships)
        public Teacher? Teacher { get; set; }
        public Student? Student { get; set; }
    }
}
   