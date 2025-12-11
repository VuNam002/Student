namespace Student_management.Models.Base
{
    public abstract class BaseEntity
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

