using System.ComponentModel.DataAnnotations;

namespace Student_management.DTOs.Class
{
    public class CreateClass
    {
        [Required(ErrorMessage = "Ma lop la bat buoc")]
        [MaxLength(50, ErrorMessage = "Ma lop toi da 50 ky tu")]
        public string ClassCode { get; set; }

        [Required(ErrorMessage = "Ten lop la bat buoc")]
        [MaxLength(20, ErrorMessage = "Ten lop toi thieu 20 ky tu")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Khoa la bat buoc")]
        public int DepartmentId { get; set; }

        public int? TeacherId { get; set; }

        [Required(ErrorMessage = "Nam hoc la bat buoc")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Nam hoc phai co dinh dang YYYY-YYYY")]
        public string AcademicYear { get; set; }

        [Range(1, 8, ErrorMessage = "Học kỳ phải từ 1 đến 8")]
        public int Semester { get; set; }
    }
}
