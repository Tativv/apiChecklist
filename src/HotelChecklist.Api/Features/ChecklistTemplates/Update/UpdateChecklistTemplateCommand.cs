using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Update;

public sealed record UpdateChecklistTemplateCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid AreaId,
    int EstimatedDurationMinutes,
    string ExecutionMode,
    List<ScheduleInput> Schedules,
    List<ChecklistTaskRequest> Tasks,
    UserRole ActingUserRole) : ICommand<UpdateChecklistTemplateResponse>;
