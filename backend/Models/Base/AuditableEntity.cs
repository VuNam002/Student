using System.ComponentModel.DataAnnotations;

namespace Student_management.Models.Base
{
    public abstract class AuditableEntity : BaseEntity
    {
        [StringLength(100)]
        public string? UpdateBy { get; set; }
        [StringLength(100)]
        public string? CreateBy { get; set; }
    }
}
