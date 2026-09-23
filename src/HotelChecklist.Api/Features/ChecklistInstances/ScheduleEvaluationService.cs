using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistInstances;

public sealed class ScheduleEvaluationService
{
    public bool ShouldExecute(Schedule schedule, DateOnly date)
    {
        if (!schedule.Active)
            return false;

        var anchor = DateOnly.FromDateTime(schedule.CreatedAtUtc.UtcDateTime);

        if (date < anchor)
            return false;

        var interval = Math.Max(schedule.IntervalValue, 1);

        return schedule.FrequencyType switch
        {
            ScheduleFrequencyType.Daily => DaysSinceAnchor(anchor, date) % interval == 0,
            ScheduleFrequencyType.Weekly => schedule.WeekDay == date.DayOfWeek
                && (DaysSinceAnchor(anchor, date) / 7) % interval == 0,
            ScheduleFrequencyType.Monthly => IsMonthlyMatch(schedule, anchor, date, interval),
            _ => false
        };
    }

    public bool ShouldExecuteTemplate(ChecklistTemplate template, DateOnly date) =>
        template.ExecutionMode == TaskExecutionMode.Continuous
            ? date >= DateOnly.FromDateTime(template.CreatedAtUtc.UtcDateTime)
            : template.TemplateSchedules.Any(ts => ShouldExecute(ts.Schedule, date));

    private static int DaysSinceAnchor(DateOnly anchor, DateOnly date) => date.DayNumber - anchor.DayNumber;

    private static bool IsMonthlyMatch(Schedule schedule, DateOnly anchor, DateOnly date, int interval)
    {
        var monthsSinceAnchor = (date.Year - anchor.Year) * 12 + (date.Month - anchor.Month);

        if (monthsSinceAnchor < 0 || monthsSinceAnchor % interval != 0)
            return false;

        var targetDay = Math.Min(schedule.DayOfMonth ?? anchor.Day, DateTime.DaysInMonth(date.Year, date.Month));
        return date.Day == targetDay;
    }
}
