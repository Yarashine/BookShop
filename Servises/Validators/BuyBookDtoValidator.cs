using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class BuyBookDtoValidator : AbstractValidator<BuyBookDto>
{
    public BuyBookDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");
    }
}