using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public static class ListCallCommentsEndpoint
{
    public static void MapListCallComments(this RouteGroupBuilder group)
    {
        group.MapGet("/{callId:guid}/comments", async (
                Guid callId,
                IQueryHandler<ListCallCommentsQuery, IReadOnlyList<CallCommentResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListCallCommentsQuery(callId), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListCallComments")
            .Produces<IReadOnlyList<CallCommentResponseItem>>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
