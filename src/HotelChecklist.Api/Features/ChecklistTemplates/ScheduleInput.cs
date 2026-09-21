namespace HotelChecklist.Api.Features.ChecklistTemplates;

public sealed record ScheduleInput(
    string FrequencyType,
    int IntervalValue,
    string? WeekDay,
    int? DayOfMonth,
    string TimeOfDay,
    int ExecutionOrder);
