using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed class GetCallCommentFileHandler(AppDbContext db, IFileStorage fileStorage) : IQueryHandler<GetCallCommentFileQuery, GetCallCommentFileResponse>
{
    public async Task<Result<GetCallCommentFileResponse>> Handle(GetCallCommentFileQuery query, CancellationToken cancellationToken)
    {
        var comment = await db.CallComments.FindAsync([query.CommentId], cancellationToken);

        if (comment is null || comment.FilePath is null)
            return Result.Failure<GetCallCommentFileResponse>(Error.NotFound("CallComments.FileNotFound", "Arquivo no encontrado."));

        var stream = await fileStorage.ReadAsync(comment.FilePath, cancellationToken);

        return Result.Success(new GetCallCommentFileResponse(stream, comment.ContentType!, comment.FileName!));
    }
}
