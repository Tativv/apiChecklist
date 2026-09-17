using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistInstances;

public static class ChecklistTemplateRecurrenceEvaluator
{
    public static bool ShouldGenerate(ChecklistTemplate template, DateOnly date)
    {
        if (date < template.RecurrenceStartDate)
            return false;

        return template.RecurrenceType switch
        {
            ChecklistRecurrenceType.Daily => true,
            ChecklistRecurrenceType.Weekly => DaysSinceStart(template, date) % 7 == 0,
            ChecklistRecurrenceType.Monthly => IsSameDayOfMonth(template.RecurrenceStartDate, date),
            ChecklistRecurrenceType.Custom => ShouldGenerateCustom(template, date),
            _ => false
        };
    }

    private static bool ShouldGenerateCustom(ChecklistTemplate template, DateOnly date) => template.CustomRecurrenceMode switch
    {
        CustomRecurrenceMode.Interval => ShouldGenerateInterval(template, date),
        CustomRecurrenceMode.DaysOfWeek => IsDayOfWeekSelected(template.RecurrenceDaysOfWeekMask, date.DayOfWeek),
        _ => false
    };

    private static bool ShouldGenerateInterval(ChecklistTemplate template, DateOnly date)
    {
        if (template.RecurrenceIntervalValue is not > 0 || template.RecurrenceIntervalUnit is null)
            return false;

        var interval = template.RecurrenceIntervalValue.Value;

        return template.RecurrenceIntervalUnit switch
        {
            RecurrenceIntervalUnit.Days => DaysSinceStart(template, date) % interval == 0,
            RecurrenceIntervalUnit.Weeks => DaysSinceStart(template, date) % (interval * 7) == 0,
            RecurrenceIntervalUnit.Months => IsMonthlyInterval(template.RecurrenceStartDate, date, interval),
            _ => false
        };
    }

    private static int DaysSinceStart(ChecklistTemplate template, DateOnly date) =>
        date.DayNumber - template.RecurrenceStartDate.DayNumber;

    private static bool IsMonthlyInterval(DateOnly start, DateOnly date, int interval)
    {
        var monthsSinceStart = (date.Year - start.Year) * 12 + (date.Month - start.Month);

        return monthsSinceStart >= 0 && monthsSinceStart % interval == 0 && IsSameDayOfMonth(start, date);
    }

    private static bool IsSameDayOfMonth(DateOnly start, DateOnly date)
    {
        var targetDay = Math.Min(start.Day, DateTime.DaysInMonth(date.Year, date.Month));
        return date.Day == targetDay;
    }

    private static bool IsDayOfWeekSelected(int? mask, DayOfWeek dayOfWeek) =>
        mask is not null && (mask.Value & (1 << (int)dayOfWeek)) != 0;
}
