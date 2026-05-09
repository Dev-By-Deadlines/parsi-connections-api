namespace Connecions.Api.Validators;

using Connecions.Api.Dtos;
using FluentValidation;

public class CreatePuzzleDtoValidator : AbstractValidator<CreatePuzzleDto>
{
    public CreatePuzzleDtoValidator()
    {
        RuleFor(x => x.Categories)
            .NotNull()
            .Must(categories => categories.Count == 4)
            .WithMessage("A puzzle must have exactly 4 categories.");

        RuleForEach(x => x.Categories)
            .SetValidator(new CreateCategoryDtoValidator());
    }
}
