using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates;

public static class ChecklistTemplateSchedulingMapping
{
    public static void ApplyScheduling(
        ChecklistTemplate template,
        string scheduledTime,
        DateOnly? recurrenceStartDate,
        string? customRecurrenceMode,
        int? recurrenceIntervalValue,
        string? recurrenceIntervalUnit,
        List<string>? recurrenceDaysOfWeek)
    {
        var mode = customRecurrenceMode is null
            ? (CustomRecurrenceMode?)null
            : Enum.Parse<CustomRecurrenceMode>(customRecurrenceMode, ignoreCase: true);

        template.ScheduledTime = ParseScheduledTime(scheduledTime);
        template.RecurrenceStartDate = recurrenceStartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        template.CustomRecurrenceMode = mode;

        template.RecurrenceIntervalValue = mode == CustomRecurrenceMode.Interval ? recurrenceIntervalValue : null;
        template.RecurrenceIntervalUnit = mode == CustomRecurrenceMode.Interval && recurrenceIntervalUnit is not null
            ? Enum.Parse<RecurrenceIntervalUnit>(recurrenceIntervalUnit, ignoreCase: true)
            : null;
        template.RecurrenceDaysOfWeekMask = mode == CustomRecurrenceMode.DaysOfWeek && recurrenceDaysOfWeek is not null
            ? ToDaysOfWeekMask(recurrenceDaysOfWeek)
            : null;
    }

    private const string TimeFormat = "HH:mm";

    public static TimeOnly ParseScheduledTime(string value) =>
        TimeOnly.ParseExact(value, TimeFormat);

    public static string FormatScheduledTime(TimeOnly value) =>
        value.ToString(TimeFormat);

    public static bool IsValidScheduledTime(string value) =>
        TimeOnly.TryParseExact(value, TimeFormat, out _);

    public static int ToDaysOfWeekMask(IEnumerable<string> days) =>
        days.Aggregate(0, (mask, day) => mask | (1 << (int)Enum.Parse<DayOfWeek>(day, ignoreCase: true)));

    public static bool AreValidDaysOfWeek(IEnumerable<string> days) =>
        days.All(d => Enum.TryParse<DayOfWeek>(d, ignoreCase: true, out _));

    public static List<string> ToDaysOfWeekList(int? mask)
    {
        if (mask is null)
            return [];

        return Enum.GetValues<DayOfWeek>()
            .Where(d => (mask.Value & (1 << (int)d)) != 0)
            .Select(d => d.ToString())
            .ToList();
    }
}
