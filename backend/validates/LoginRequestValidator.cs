using FluentValidation;
using Student_management.DTOs.Account;

namespace Student_management.validates
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email la bat buoc")
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                .MaximumLength(50).WithMessage("Mật khẩu không được vượt quá 50 ký tự");
        }
    }
}
