using FluentValidation;
using Models.Dtos;

namespace Servises.Validators;

public class BookInfoDtoValidator : AbstractValidator<BookInfoDto>
{
    public BookInfoDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.AuthorName).NotEmpty().WithMessage("Author name is required.");
        RuleFor(x => x.DateOfPublication).NotEmpty().WithMessage("Date of publication is required.");
        RuleFor(x => x.Likes).GreaterThanOrEqualTo(0).WithMessage("Likes must be greater than or equal to 0.");
    }
}