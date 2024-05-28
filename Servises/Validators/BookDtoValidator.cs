using Models.Dtos;
using FluentValidation;
namespace Servises.Validators;
public class BookDtoValidator : AbstractValidator<BookDto>
{
    public BookDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");
    }
}