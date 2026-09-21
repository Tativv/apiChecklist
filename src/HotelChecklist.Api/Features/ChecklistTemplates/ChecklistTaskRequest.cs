namespace HotelChecklist.Api.Features.ChecklistTemplates;

public sealed record ChecklistTaskRequest(
    string Name,
    string? Description,
    int Order,
    string ExecutionMode,
    List<ScheduleInput> Schedules);
