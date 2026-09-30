using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public static class GetServiceOrderCommentFileEndpoint
{
    public static void MapGetServiceOrderCommentFile(this RouteGroupBuilder group)
    {
        group.MapGet("/comments/{commentId:guid}/file", async (
                Guid commentId,
                IQueryHandler<GetServiceOrderCommentFileQuery, GetServiceOrderCommentFileResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetServiceOrderCommentFileQuery(commentId), cancellationToken);

                if (result.IsFailure)
                    return result.ToHttpResult();

                return Microsoft.AspNetCore.Http.Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetServiceOrderCommentFile")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
