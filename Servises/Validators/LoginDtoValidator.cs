using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}