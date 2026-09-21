using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates;

public sealed class ScheduleInputValidator : AbstractValidator<ScheduleInput>
{
    public ScheduleInputValidator()
    {
        RuleFor(s => s.FrequencyType)
            .NotEmpty()
            .Must(f => Enum.TryParse<ScheduleFrequencyType>(f, ignoreCase: true, out _))
            .WithMessage($"FrequencyType must be one of: {string.Join(", ", Enum.GetNames<ScheduleFrequencyType>())}.");

        RuleFor(s => s.IntervalValue).GreaterThanOrEqualTo(1);

        RuleFor(s => s.TimeOfDay)
            .NotEmpty()
            .Must(ScheduleMapping.IsValidTimeOfDay)
            .WithMessage("TimeOfDay must be in HH:mm format.");

        RuleFor(s => s.WeekDay)
            .Must(d => d is null || Enum.TryParse<DayOfWeek>(d, ignoreCase: true, out _))
            .WithMessage("WeekDay must be a valid day name (e.g. Monday).");

        RuleFor(s => s.DayOfMonth)
            .InclusiveBetween(1, 31)
            .When(s => s.DayOfMonth is not null);

        When(s => string.Equals(s.FrequencyType, nameof(ScheduleFrequencyType.Weekly), StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(s => s.WeekDay).NotEmpty().WithMessage("Weekly schedules require WeekDay.");
        });

        When(s => string.Equals(s.FrequencyType, nameof(ScheduleFrequencyType.Monthly), StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(s => s.DayOfMonth).NotNull().WithMessage("Monthly schedules require DayOfMonth.");
        });
    }
}
