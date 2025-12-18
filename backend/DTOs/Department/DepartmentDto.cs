namespace Student_management.DTOs.Department
{
    public class DepartmentDto
    {
        public int DepartmentID { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
