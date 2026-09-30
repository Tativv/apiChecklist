using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed class GetServiceOrderCommentFileHandler(AppDbContext db, IFileStorage fileStorage)
    : IQueryHandler<GetServiceOrderCommentFileQuery, GetServiceOrderCommentFileResponse>
{
    public async Task<Result<GetServiceOrderCommentFileResponse>> Handle(GetServiceOrderCommentFileQuery query, CancellationToken cancellationToken)
    {
        var comment = await db.ServiceOrderComments.FindAsync([query.CommentId], cancellationToken);

        if (comment is null || comment.FilePath is null)
            return Result.Failure<GetServiceOrderCommentFileResponse>(Error.NotFound("ServiceOrderComments.FileNotFound", "Arquivo no encontrado."));

        var stream = await fileStorage.ReadAsync(comment.FilePath, cancellationToken);

        return Result.Success(new GetServiceOrderCommentFileResponse(stream, comment.ContentType!, comment.FileName!));
    }
}
