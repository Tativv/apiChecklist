namespace HotelChecklist.Api.Features.ChecklistTemplates;

public sealed record ChecklistTaskResponseItem(
    Guid Id,
    string Name,
    string? Description,
    int Order,
    string ExecutionMode,
    List<ScheduleResponseItem> Schedules);
