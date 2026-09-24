using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed class AddTaskCommentRequestValidator : AbstractValidator<AddTaskCommentRequest>
{
    public AddTaskCommentRequestValidator()
    {
        RuleFor(r => r.Text).NotEmpty().MaximumLength(2000);
    }
}
