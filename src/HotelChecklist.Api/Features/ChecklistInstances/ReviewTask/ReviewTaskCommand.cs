using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.ReviewTask;

public sealed record ReviewTaskCommand(Guid InstanceId, Guid TaskExecutionId, Guid ActingUserId) : ICommand<ReviewTaskResponse>;
