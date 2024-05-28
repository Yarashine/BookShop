using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class FilterDtoValidator : AbstractValidator<FilterDto>
{
    public FilterDtoValidator()
    {
        RuleFor(x => x.Sort).GreaterThanOrEqualTo(0).WithMessage("Sort must be greater than or equal to 0.");
    }
}