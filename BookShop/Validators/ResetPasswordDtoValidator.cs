using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required.");
        RuleFor(x => x.ResetToken).NotEmpty().WithMessage("Reset token is required.");
    }
}