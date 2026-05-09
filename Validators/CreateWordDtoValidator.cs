namespace Connecions.Api.Validators;

using Connecions.Api.Dtos;
using FluentValidation;

public class CreateWordDtoValidator : AbstractValidator<CreateWordDto>
{
    public CreateWordDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Word text cannot be empty.");
    }
}
