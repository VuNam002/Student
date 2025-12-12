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
                .EmailAddress().WithMessage("Email khong hop le")
                .MaximumLength(100).WithMessage("Email khong ton tai");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mat khau la bat buoc")
                .MinimumLength(6).WithMessage("Mat khau phai it nhat 6 ky tu")
                .MaximumLength(50).WithMessage("Mat khau khong vuot qua 50 ky tu");
        }
    }
}
