using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed record UpdateChecklistTemplateCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    int EstimatedDurationMinutes,
    List<ScheduleInput> Schedules,
    List<ChecklistTaskRequest> Tasks) : ICommand<UpdateChecklistTemplateResponse>;
