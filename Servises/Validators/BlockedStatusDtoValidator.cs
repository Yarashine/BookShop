﻿using FluentValidation;
using Models.Dtos;
namespace Servises.Validators;
public class BlockedStatusDtoValidator : AbstractValidator<BlockedStatusDto>
{
    public BlockedStatusDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
    }
}