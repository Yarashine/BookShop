using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class CommentDtoValidator : AbstractValidator<CommentDto>
{
    public CommentDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
    }
}