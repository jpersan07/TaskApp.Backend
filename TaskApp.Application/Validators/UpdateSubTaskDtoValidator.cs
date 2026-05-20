using FluentValidation;
using TaskApp.Application.DTOs;

namespace TaskApp.Application.Validators;

/// <summary>
/// Validator for UpdateSubTaskDto — enforces rules on incoming subtask update requests
/// </summary>
public class UpdateSubTaskDtoValidator : AbstractValidator<UpdateSubTaskDto>
{
    public UpdateSubTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.")
            .When(x => x.Title != null);
    }
}
