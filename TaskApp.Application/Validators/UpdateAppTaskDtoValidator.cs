using FluentValidation;
using TaskApp.Application.DTOs;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validator for UpdateAppTaskDto — enforces rules on incoming task update requests
/// </summary>
public class UpdateAppTaskDtoValidator : AbstractValidator<UpdateAppTaskDto>
{
    public UpdateAppTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(50).WithMessage("Title must not exceed 50 characters.")
            .When(x => x.Title != null);

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("DueDate cannot be in the past.")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.DueDate)
            .NotNull().WithMessage("DueDate is required when Status is Finished.")
            .When(x => x.Status == TaskStatus.Finished);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be a valid id.")
            .When(x => x.CategoryId.HasValue);
    }
}
