using FluentValidation;
using TaskApp.Application.DTOs;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validator for CreateCategoryDto — enforces rules on incoming category creation requests
/// </summary>
public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
    }
}
