using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Assets.List;

public sealed class ListAssetsHandler(AppDbContext db) : IQueryHandler<ListAssetsQuery, IReadOnlyList<ListAssetsResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListAssetsResponseItem>>> Handle(ListAssetsQuery query, CancellationToken cancellationToken)
    {
        var assetsQuery = db.Assets.AsQueryable();

        if (query.AreaId is not null)
            assetsQuery = assetsQuery.Where(a => a.AreaId == query.AreaId);

        if (query.Active is not null)
            assetsQuery = assetsQuery.Where(a => a.Active == query.Active);

        var assets = await assetsQuery
            .OrderBy(a => a.Name)
            .Select(a => new ListAssetsResponseItem(a.Id, a.Name, a.Type, a.AreaId, a.Active))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListAssetsResponseItem>>(assets);
    }
}
