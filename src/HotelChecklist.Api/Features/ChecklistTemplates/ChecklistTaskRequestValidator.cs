using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates;

public sealed class ChecklistTaskRequestValidator : AbstractValidator<ChecklistTaskRequest>
{
    public ChecklistTaskRequestValidator()
    {
        RuleFor(t => t.Name).NotEmpty().MaximumLength(200);
        RuleFor(t => t.Description).MaximumLength(1000);
        RuleFor(t => t.Order).GreaterThanOrEqualTo(0);

        RuleFor(t => t.ExecutionMode)
            .NotEmpty()
            .Must(m => Enum.TryParse<TaskExecutionMode>(m, ignoreCase: true, out _))
            .WithMessage($"ExecutionMode must be one of: {string.Join(", ", Enum.GetNames<TaskExecutionMode>())}.");

        RuleForEach(t => t.Schedules).SetValidator(new ScheduleInputValidator());

        When(t => string.Equals(t.ExecutionMode, nameof(TaskExecutionMode.Scheduled), StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(t => t.Schedules).NotEmpty().WithMessage("Scheduled tasks require at least one schedule.");
        });

        When(t => string.Equals(t.ExecutionMode, nameof(TaskExecutionMode.Continuous), StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(t => t.Schedules).Empty().WithMessage("Continuous tasks must not have schedules.");
        });
    }
}
