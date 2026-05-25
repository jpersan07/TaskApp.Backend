using FluentValidation.TestHelper;
using TaskApp.Application.DTOs;
using TaskApp.Application.Validators;
using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests.Validators;

public class CreateAppTaskDtoValidatorTests
{
    private readonly CreateAppTaskDtoValidator _validator = new();

    [Fact]
    public void ValidTitle_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "My Task" });
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void EmptyTitle_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "" });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title is required.");
    }

    [Fact]
    public void TitleOver50Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = new string('x', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title must not exceed 50 characters.");
    }

    [Fact]
    public void NullDueDate_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "Task", DueDate = null });
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void FutureDueDate_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "Task", DueDate = DateTime.Today.AddDays(1) });
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void PastDueDate_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "Task", DueDate = DateTime.Today.AddDays(-1) });
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void NullCategoryId_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "Task", CategoryId = null });
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ValidCategoryId_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "Task", CategoryId = 5 });
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ZeroCategoryId_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateAppTaskDto { Title = "Task", CategoryId = 0 });
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}

public class CreateCategoryDtoValidatorTests
{
    private readonly CreateCategoryDtoValidator _validator = new();

    [Fact]
    public void ValidName_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateCategoryDto { Name = "Work" });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateCategoryDto { Name = "" });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
    }

    [Fact]
    public void NameOver50Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateCategoryDto { Name = new string('x', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must not exceed 50 characters.");
    }
}

public class CreateSubTaskDtoValidatorTests
{
    private readonly CreateSubTaskDtoValidator _validator = new();

    [Fact]
    public void ValidTitleAndTaskId_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateSubTaskDto { Title = "Step 1", TaskId = 1 });
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyTitle_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateSubTaskDto { Title = "", TaskId = 1 });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title is required.");
    }

    [Fact]
    public void TitleOver100Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateSubTaskDto { Title = new string('x', 101), TaskId = 1 });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title must not exceed 100 characters.");
    }

    [Fact]
    public void ZeroTaskId_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateSubTaskDto { Title = "Step", TaskId = 0 });
        result.ShouldHaveValidationErrorFor(x => x.TaskId);
    }
}

public class CreateTagDtoValidatorTests
{
    private readonly CreateTagDtoValidator _validator = new();

    [Fact]
    public void ValidName_ShouldPass()
    {
        var result = _validator.TestValidate(new CreateTagDto { Name = "Urgent" });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateTagDto { Name = "" });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
    }

    [Fact]
    public void NameOver50Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new CreateTagDto { Name = new string('x', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must not exceed 50 characters.");
    }
}

public class UpdateAppTaskDtoValidatorTests
{
    private readonly UpdateAppTaskDtoValidator _validator = new();

    [Fact]
    public void NullTitle_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { Title = null });
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void ValidTitle_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { Title = "Updated" });
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void EmptyTitle_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { Title = "" });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title cannot be empty.");
    }

    [Fact]
    public void TitleOver50Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { Title = new string('x', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title must not exceed 50 characters.");
    }

    [Fact]
    public void NullDueDate_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { DueDate = null });
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void FutureDueDate_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { DueDate = DateTime.Today.AddDays(1) });
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void PastDueDate_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { DueDate = DateTime.Today.AddDays(-1) });
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void StatusFinishedWithNullDueDate_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { Status = TaskStatus.Finished, DueDate = null });
        result.ShouldHaveValidationErrorFor(x => x.DueDate).WithErrorMessage("DueDate is required when Status is Finished.");
    }

    [Fact]
    public void StatusFinishedWithDueDate_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { Status = TaskStatus.Finished, DueDate = DateTime.Today.AddDays(1) });
        result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void NullCategoryId_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { CategoryId = null });
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ValidCategoryId_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { CategoryId = 3 });
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ZeroCategoryId_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateAppTaskDto { CategoryId = 0 });
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}

public class UpdateCategoryDtoValidatorTests
{
    private readonly UpdateCategoryDtoValidator _validator = new();

    [Fact]
    public void NullName_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateCategoryDto { Name = null });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidName_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateCategoryDto { Name = "Personal" });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateCategoryDto { Name = "" });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name cannot be empty.");
    }

    [Fact]
    public void NameOver50Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateCategoryDto { Name = new string('x', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must not exceed 50 characters.");
    }
}

public class UpdateSubTaskDtoValidatorTests
{
    private readonly UpdateSubTaskDtoValidator _validator = new();

    [Fact]
    public void NullTitle_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateSubTaskDto { Title = null });
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void ValidTitle_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateSubTaskDto { Title = "Do the thing" });
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void EmptyTitle_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateSubTaskDto { Title = "" });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title cannot be empty.");
    }

    [Fact]
    public void TitleOver100Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateSubTaskDto { Title = new string('x', 101) });
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title must not exceed 100 characters.");
    }
}

public class UpdateTagDtoValidatorTests
{
    private readonly UpdateTagDtoValidator _validator = new();

    [Fact]
    public void NullName_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateTagDto { Name = null });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidName_ShouldPass()
    {
        var result = _validator.TestValidate(new UpdateTagDto { Name = "Bug" });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateTagDto { Name = "" });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name cannot be empty.");
    }

    [Fact]
    public void NameOver50Chars_ShouldFail()
    {
        var result = _validator.TestValidate(new UpdateTagDto { Name = new string('x', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must not exceed 50 characters.");
    }
}
