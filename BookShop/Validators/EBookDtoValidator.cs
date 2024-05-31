using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class EBookDtoValidator : AbstractValidator<EBookDto>
{
    public EBookDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}