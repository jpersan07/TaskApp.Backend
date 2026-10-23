using FluentValidation;
using TaskApp.Application.DTOs;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validator for CreateAppTaskDto — enforces rules on incoming task creation requests
/// </summary>
public class CreateAppTaskDtoValidator : AbstractValidator<CreateAppTaskDto>
{
    public CreateAppTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(50).WithMessage("Title must not exceed 50 characters.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("DueDate cannot be in the past.")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be a valid id.")
            .When(x => x.CategoryId.HasValue);
    }
}
