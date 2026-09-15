using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistInstances.Reopen;

public sealed class ReopenChecklistInstanceRequestValidator : AbstractValidator<ReopenChecklistInstanceRequest>
{
    public ReopenChecklistInstanceRequestValidator()
    {
        RuleFor(r => r.Reason).MaximumLength(500);
    }
}
