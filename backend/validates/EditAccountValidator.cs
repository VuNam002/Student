using FluentValidation;
using Student_management.DTOs.Account;

namespace Student_management.Validators
{
    public class EditAccountValidator : AbstractValidator<CreateAccount>
    {
        public EditAccountValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email khong duoc de trong")
                .EmailAddress().WithMessage("Email khong hop le");

            RuleFor(x => x.Password)
                .MinimumLength(6).When(x => !string.IsNullOrEmpty(x.Password))
                .WithMessage("Mat khau phai co it nhat 6 ky tu");
        }
    }
}