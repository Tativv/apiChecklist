using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed class ListCallCommentsHandler(AppDbContext db) : IQueryHandler<ListCallCommentsQuery, IReadOnlyList<CallCommentResponseItem>>
{
    public async Task<Result<IReadOnlyList<CallCommentResponseItem>>> Handle(ListCallCommentsQuery query, CancellationToken cancellationToken)
    {
        var callExists = await db.Calls.AnyAsync(c => c.Id == query.CallId, cancellationToken);

        if (!callExists)
            return Result.Failure<IReadOnlyList<CallCommentResponseItem>>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        var comments = await db.CallComments
            .Where(c => c.CallId == query.CallId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CallCommentResponseItem(c.Id, c.AuthorUserId, c.AuthorUser.Name, c.CreatedAt, c.Text, c.FileName, c.ContentType))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CallCommentResponseItem>>(comments);
    }
}
