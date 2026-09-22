using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public sealed record CompleteChecklistTaskCommand(
    Guid InstanceId,
    Guid TaskExecutionId,
    bool Completed,
    string? Comment,
    Guid ActingUserId,
    bool ActingUserIsManagerOrAbove) : ICommand<CompleteChecklistTaskResponse>;
