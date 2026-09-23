using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Create;

public sealed record CreateChecklistTemplateCommand(
    string Name,
    string? Description,
    Guid AreaId,
    int EstimatedDurationMinutes,
    string ExecutionMode,
    List<ScheduleInput> Schedules,
    List<ChecklistTaskRequest> Tasks) : ICommand<CreateChecklistTemplateResponse>;
