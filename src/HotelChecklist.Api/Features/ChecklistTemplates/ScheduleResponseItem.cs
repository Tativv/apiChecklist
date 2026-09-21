namespace HotelChecklist.Api.Features.ChecklistTemplates;

public sealed record ScheduleResponseItem(
    Guid Id,
    string FrequencyType,
    int IntervalValue,
    string? WeekDay,
    int? DayOfMonth,
    string TimeOfDay,
    int ExecutionOrder,
    bool Active);
