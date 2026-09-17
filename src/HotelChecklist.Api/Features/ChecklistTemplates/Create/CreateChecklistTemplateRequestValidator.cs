using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed class CreateChecklistTemplateRequestValidator : AbstractValidator<CreateChecklistTemplateRequest>
{
    public CreateChecklistTemplateRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(1000);
        RuleFor(r => r.AreaId).NotEmpty();
        RuleFor(r => r.RecurrenceType)
            .NotEmpty()
            .Must(rt => Enum.TryParse<ChecklistRecurrenceType>(rt, ignoreCase: true, out _))
            .WithMessage($"RecurrenceType must be one of: {string.Join(", ", Enum.GetNames<ChecklistRecurrenceType>())}.");
        RuleFor(r => r.EstimatedDurationMinutes).GreaterThan(0);
        RuleFor(r => r.ScheduledTime)
            .NotEmpty()
            .Must(ChecklistTemplateSchedulingMapping.IsValidScheduledTime)
            .WithMessage("ScheduledTime must be in HH:mm format.");

        When(r => string.Equals(r.RecurrenceType, nameof(ChecklistRecurrenceType.Custom), StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(r => r.CustomRecurrenceMode)
                .NotEmpty()
                .Must(m => m is null || Enum.TryParse<CustomRecurrenceMode>(m, ignoreCase: true, out _))
                .WithMessage($"CustomRecurrenceMode must be one of: {string.Join(", ", Enum.GetNames<CustomRecurrenceMode>())}.");

            When(r => string.Equals(r.CustomRecurrenceMode, nameof(CustomRecurrenceMode.Interval), StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(r => r.RecurrenceIntervalValue).NotNull().GreaterThan(0);
                RuleFor(r => r.RecurrenceIntervalUnit)
                    .NotEmpty()
                    .Must(u => u is null || Enum.TryParse<RecurrenceIntervalUnit>(u, ignoreCase: true, out _))
                    .WithMessage($"RecurrenceIntervalUnit must be one of: {string.Join(", ", Enum.GetNames<RecurrenceIntervalUnit>())}.");
            });

            When(r => string.Equals(r.CustomRecurrenceMode, nameof(CustomRecurrenceMode.DaysOfWeek), StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(r => r.RecurrenceDaysOfWeek).NotEmpty();
                RuleForEach(r => r.RecurrenceDaysOfWeek)
                    .Must(d => Enum.TryParse<DayOfWeek>(d, ignoreCase: true, out _))
                    .WithMessage("Each day in RecurrenceDaysOfWeek must be a valid day name (e.g. Monday).");
            });
        });

        RuleFor(r => r.Tasks).NotEmpty();
        RuleForEach(r => r.Tasks).ChildRules(task =>
        {
            task.RuleFor(t => t.Name).NotEmpty().MaximumLength(200);
            task.RuleFor(t => t.Description).MaximumLength(1000);
            task.RuleFor(t => t.Order).GreaterThanOrEqualTo(0);
        });
    }
}
