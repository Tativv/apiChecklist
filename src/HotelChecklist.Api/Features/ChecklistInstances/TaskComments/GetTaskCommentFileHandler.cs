using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed class GetTaskCommentFileHandler(AppDbContext db, IFileStorage fileStorage) : IQueryHandler<GetTaskCommentFileQuery, GetTaskCommentFileResponse>
{
    public async Task<Result<GetTaskCommentFileResponse>> Handle(GetTaskCommentFileQuery query, CancellationToken cancellationToken)
    {
        var comment = await db.ChecklistTaskComments.FindAsync([query.CommentId], cancellationToken);

        if (comment is null || comment.FilePath is null)
            return Result.Failure<GetTaskCommentFileResponse>(Error.NotFound("ChecklistTaskComments.FileNotFound", "Arquivo no encontrado."));

        var stream = await fileStorage.ReadAsync(comment.FilePath, cancellationToken);

        return Result.Success(new GetTaskCommentFileResponse(stream, comment.ContentType!, comment.FileName!));
    }
}
