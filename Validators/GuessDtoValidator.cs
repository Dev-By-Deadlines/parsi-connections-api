namespace Connecions.Api.Validators;

using Connecions.Api.Dtos;
using FluentValidation;

public class GuessDtoValidator : AbstractValidator<GuessDto>
{
    public GuessDtoValidator()
    {
        RuleFor(x => x.Words)
            .Must(w => w.Count == 4)
            .WithMessage("A guess must contain exactly 4 words.");

        RuleFor(x => x.Words)
            .Must(w => w.Distinct().Count() == 4)
            .WithMessage("All words in a guess must be different.");
    }
}
