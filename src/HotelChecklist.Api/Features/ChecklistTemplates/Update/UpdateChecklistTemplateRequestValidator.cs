using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed class UpdateChecklistTemplateRequestValidator : AbstractValidator<UpdateChecklistTemplateRequest>
{
    public UpdateChecklistTemplateRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(1000);
        RuleFor(r => r.AreaId).NotEmpty();
        RuleFor(r => r.RecurrenceType)
            .NotEmpty()
            .Must(rt => Enum.TryParse<ChecklistRecurrenceType>(rt, ignoreCase: true, out _))
            .WithMessage($"RecurrenceType must be one of: {string.Join(", ", Enum.GetNames<ChecklistRecurrenceType>())}.");
        RuleFor(r => r.EstimatedDurationMinutes).GreaterThan(0);
        RuleFor(r => r.Tasks).NotEmpty();
        RuleForEach(r => r.Tasks).ChildRules(task =>
        {
            task.RuleFor(t => t.Name).NotEmpty().MaximumLength(200);
            task.RuleFor(t => t.Order).GreaterThanOrEqualTo(0);
        });
    }
}
