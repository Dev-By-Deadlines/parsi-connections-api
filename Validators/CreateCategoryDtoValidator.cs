namespace Connecions.Api.Validators;

using Connecions.Api.Dtos;
using FluentValidation;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Words)
            .Must(words => words.Count == 4)
            .WithMessage("Each Category must have exactly 4 words.");

        RuleForEach(x => x.Words)
            .SetValidator(new CreateWordDtoValidator());
    }
}
