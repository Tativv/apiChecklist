using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public static class GetTaskCommentFileEndpoint
{
    public static void MapGetTaskCommentFile(this RouteGroupBuilder group)
    {
        group.MapGet("/comments/{commentId:guid}/file", async (
                Guid commentId,
                IQueryHandler<GetTaskCommentFileQuery, GetTaskCommentFileResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetTaskCommentFileQuery(commentId), cancellationToken);

                if (result.IsFailure)
                    return result.ToHttpResult();

                return Microsoft.AspNetCore.Http.Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetTaskCommentFile")
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
