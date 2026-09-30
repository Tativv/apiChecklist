using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public static class ListServiceOrderCommentsEndpoint
{
    public static void MapListServiceOrderComments(this RouteGroupBuilder group)
    {
        group.MapGet("/{serviceOrderId:guid}/comments", async (
                Guid serviceOrderId,
                IQueryHandler<ListServiceOrderCommentsQuery, IReadOnlyList<ServiceOrderCommentResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ListServiceOrderCommentsQuery(serviceOrderId), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("ListServiceOrderComments")
            .Produces<IReadOnlyList<ServiceOrderCommentResponseItem>>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
