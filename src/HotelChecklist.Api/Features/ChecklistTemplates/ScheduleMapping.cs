using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates;

public static class ScheduleMapping
{
    private const string TimeFormat = "HH:mm";

    public static TimeOnly ParseTimeOfDay(string value) =>
        TimeOnly.ParseExact(value, TimeFormat);

    public static string FormatTimeOfDay(TimeOnly value) =>
        value.ToString(TimeFormat);

    public static bool IsValidTimeOfDay(string value) =>
        TimeOnly.TryParseExact(value, TimeFormat, out _);

    public static Schedule ToSchedule(this ScheduleInput input) => new()
    {
        Id = Guid.NewGuid(),
        FrequencyType = Enum.Parse<ScheduleFrequencyType>(input.FrequencyType, ignoreCase: true),
        IntervalValue = Math.Max(input.IntervalValue, 1),
        WeekDay = input.WeekDay is null ? null : Enum.Parse<DayOfWeek>(input.WeekDay, ignoreCase: true),
        DayOfMonth = input.DayOfMonth,
        TimeOfDay = ParseTimeOfDay(input.TimeOfDay),
        ExecutionOrder = input.ExecutionOrder,
        Active = true,
        CreatedAtUtc = DateTimeOffset.UtcNow
    };

    public static ScheduleResponseItem ToResponseItem(this Schedule schedule) => new(
        schedule.Id,
        schedule.FrequencyType.ToString(),
        schedule.IntervalValue,
        schedule.WeekDay?.ToString(),
        schedule.DayOfMonth,
        FormatTimeOfDay(schedule.TimeOfDay),
        schedule.ExecutionOrder,
        schedule.Active);
}
