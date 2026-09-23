using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Areas.List;

public sealed class ListAreasHandler(AppDbContext db) : IQueryHandler<ListAreasQuery, IReadOnlyList<ListAreasResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListAreasResponseItem>>> Handle(ListAreasQuery query, CancellationToken cancellationToken)
    {
        var areas = await db.Areas
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(a => new ListAreasResponseItem(a.Id, a.Name))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListAreasResponseItem>>(areas);
    }
}
