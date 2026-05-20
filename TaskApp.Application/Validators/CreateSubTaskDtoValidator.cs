using FluentValidation;
using TaskApp.Application.DTOs;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validator for CreateSubTaskDto — enforces rules on incoming subtask creation requests
/// </summary>
public class CreateSubTaskDtoValidator : AbstractValidator<CreateSubTaskDto>
{
    public CreateSubTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("TaskId must be a valid id.");
    }
}
