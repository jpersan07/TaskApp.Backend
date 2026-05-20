using FluentValidation;
using TaskApp.Application.DTOs;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validator for CreateTagDto — enforces rules on incoming tag creation requests
/// </summary>
public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
    }
}
