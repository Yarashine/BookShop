using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}