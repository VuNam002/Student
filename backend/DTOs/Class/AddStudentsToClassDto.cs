using System.ComponentModel.DataAnnotations;

namespace Student_management.DTOs.Class
{
    public class AddStudentsToClassDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Phai co it nhat 1 sinh vien")]
        public List<int> StudentIds { get; set; }
    }
}
