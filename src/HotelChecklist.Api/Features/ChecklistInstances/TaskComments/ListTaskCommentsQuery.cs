using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed record ListTaskCommentsQuery(Guid InstanceId, Guid TaskExecutionId) : IQuery<IReadOnlyList<TaskCommentResponseItem>>;
