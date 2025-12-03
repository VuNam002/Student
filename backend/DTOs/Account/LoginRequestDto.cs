using System.ComponentModel.DataAnnotations;

namespace Student_management.DTOs.Account
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage ="Ten dang nhap la bat buoc")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Mat khau la bat buoc")]
        public string? Password {  get; set; }
    }
}
