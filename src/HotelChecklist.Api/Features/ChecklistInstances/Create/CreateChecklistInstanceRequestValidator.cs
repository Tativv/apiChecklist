using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public sealed class CreateChecklistInstanceRequestValidator : AbstractValidator<CreateChecklistInstanceRequest>
{
    public CreateChecklistInstanceRequestValidator()
    {
        RuleFor(r => r.TemplateId).NotEmpty();
        RuleFor(r => r.AssetId).NotEmpty();
        RuleFor(r => r.Date).NotEmpty();
    }
}
