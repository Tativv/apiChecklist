using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.StartTask;

public sealed record StartTaskCommand(
    Guid InstanceId,
    Guid TaskExecutionId,
    Guid ActingUserId,
    bool ActingUserIsSupervisorOrAbove) : ICommand<StartTaskResponse>;
