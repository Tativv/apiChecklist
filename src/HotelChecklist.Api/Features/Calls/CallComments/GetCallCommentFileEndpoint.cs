using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public static class GetCallCommentFileEndpoint
{
    public static void MapGetCallCommentFile(this RouteGroupBuilder group)
    {
        group.MapGet("/comments/{commentId:guid}/file", async (
                Guid commentId,
                IQueryHandler<GetCallCommentFileQuery, GetCallCommentFileResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetCallCommentFileQuery(commentId), cancellationToken);

                if (result.IsFailure)
                    return result.ToHttpResult();

                return Microsoft.AspNetCore.Http.Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetCallCommentFile")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
