using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public sealed class CompleteChecklistTaskRequestValidator : AbstractValidator<CompleteChecklistTaskRequest>
{
    public CompleteChecklistTaskRequestValidator()
    {
        RuleFor(r => r.Comment).MaximumLength(1000);
    }
}
