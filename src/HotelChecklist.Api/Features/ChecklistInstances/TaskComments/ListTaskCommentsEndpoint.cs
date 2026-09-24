using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public static class ListTaskCommentsEndpoint
{
    public static void MapListTaskComments(this RouteGroupBuilder group)
    {
        group.MapGet("/{instanceId:guid}/tasks/{taskExecutionId:guid}/comments", async (
                Guid instanceId,
                Guid taskExecutionId,
                IQueryHandler<ListTaskCommentsQuery, IReadOnlyList<TaskCommentResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListTaskCommentsQuery(instanceId, taskExecutionId), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListTaskComments")
            .Produces<IReadOnlyList<TaskCommentResponseItem>>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
