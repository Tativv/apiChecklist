using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed class CreateChecklistTemplateRequestValidator : AbstractValidator<CreateChecklistTemplateRequest>
{
    public CreateChecklistTemplateRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(1000);
        RuleFor(r => r.AreaId).NotEmpty();
        RuleFor(r => r.EstimatedDurationMinutes).GreaterThan(0);

        RuleFor(r => r.Schedules).NotEmpty().WithMessage("A template requires at least one schedule.");
        RuleForEach(r => r.Schedules).SetValidator(new ScheduleInputValidator());

        RuleFor(r => r.Tasks).NotEmpty();
        RuleForEach(r => r.Tasks).SetValidator(new ChecklistTaskRequestValidator());
    }
}
