using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.AssignTask;

public sealed record AssignTaskCommand(
    Guid InstanceId,
    Guid TaskExecutionId,
    Guid? UserId,
    Guid ActingUserId,
    bool ActingUserIsExactlySupervisor) : ICommand<AssignTaskResponse>;
