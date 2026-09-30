using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed class ListServiceOrderCommentsHandler(AppDbContext db)
    : IQueryHandler<ListServiceOrderCommentsQuery, IReadOnlyList<ServiceOrderCommentResponseItem>>
{
    public async Task<Result<IReadOnlyList<ServiceOrderCommentResponseItem>>> Handle(ListServiceOrderCommentsQuery query, CancellationToken cancellationToken)
    {
        var serviceOrderExists = await db.ServiceOrders.AnyAsync(so => so.Id == query.ServiceOrderId, cancellationToken);

        if (!serviceOrderExists)
            return Result.Failure<IReadOnlyList<ServiceOrderCommentResponseItem>>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        var comments = await db.ServiceOrderComments
            .Where(c => c.ServiceOrderId == query.ServiceOrderId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new ServiceOrderCommentResponseItem(c.Id, c.AuthorUserId, c.AuthorUser.Name, c.CreatedAt, c.Text, c.FileName, c.ContentType))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ServiceOrderCommentResponseItem>>(comments);
    }
}
