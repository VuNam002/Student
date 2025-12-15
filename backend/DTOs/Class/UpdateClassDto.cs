
using System.ComponentModel.DataAnnotations;

namespace Student_management.DTOs.Class
{
    public class UpdateClassDto
    {
        [MaxLength(200)]
        public string ClassName { get; set; }

        public int? TeacherId { get; set; }

        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Nam hoc phai co dinh dang YYYY-YYYY")]
        public string AcademicYear { get; set; }

        [Range(1, 8, ErrorMessage = "Hoc ky phai tu 1 - 8")]
        public int? Semester
        {
            get; set;
        }
    }
}
