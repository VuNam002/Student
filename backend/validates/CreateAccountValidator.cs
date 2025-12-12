using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Account;

namespace Student_management.Validators
{
    public class CreateAccountValidator : AbstractValidator<CreateAccount>
    {
        private readonly AppDbContext _context;
        public CreateAccountValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email la bat buoc")
                .EmailAddress().WithMessage("Email khong hop le")
                .MaximumLength(100).WithMessage("Email khong duoc vuot qua 100 ky tu")
                .MustAsync(BeUniqueEmail).WithMessage("Email da ton tai trong he thong");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mat khau la bat buoc")
                .MinimumLength(6).WithMessage("mat khau phai co it nhat la 6 ky tu")
                .MaximumLength(50).WithMessage("Mau khau khong vuot qua 50 ky tu")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
                .WithMessage("Mat khau phai it nhat chua 1 chu in hoa, mot chu in thuong va mot ky tu");

            RuleFor(x => x.RoleID)
                .GreaterThan(0).WithMessage("RoleID lon hon 0")
                .MustAsync(RoleExists).WithMessage("Role khong ton tai");

            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Ho ten khong vuot qua 100 ky tu")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0\d{9}$").WithMessage("So dien thoai phai co 10 chu so va bat dau bang so 0")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Status)
                .InclusiveBetween((byte)0, (byte)1).WithMessage("Status chi co the la 0 hoac 1");
        }

        private async Task<bool> BeUniqueEmail(string? email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            return !await _context.Accounts.AnyAsync(a => a.Email == email.Trim(), cancellationToken);
        }

        private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
        {
            return await _context.Roles.AnyAsync(r => r.RoleID == roleId, cancellationToken);
        }
    }
}