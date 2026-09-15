using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.List;

public sealed class ListChecklistInstancesHandler(AppDbContext db) : IQueryHandler<ListChecklistInstancesQuery, IReadOnlyList<ListChecklistInstancesResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListChecklistInstancesResponseItem>>> Handle(ListChecklistInstancesQuery query, CancellationToken cancellationToken)
    {
        var instancesQuery = db.ChecklistInstances.AsQueryable();

        if (query.FromDate is not null)
            instancesQuery = instancesQuery.Where(i => i.Date >= query.FromDate);

        if (query.ToDate is not null)
            instancesQuery = instancesQuery.Where(i => i.Date <= query.ToDate);

        if (query.AreaId is not null)
            instancesQuery = instancesQuery.Where(i => i.Asset.AreaId == query.AreaId);

        if (query.AssetId is not null)
            instancesQuery = instancesQuery.Where(i => i.AssetId == query.AssetId);

        if (query.AssignedUserId is not null)
            instancesQuery = instancesQuery.Where(i => i.AssignedUserId == query.AssignedUserId);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<ChecklistStatus>(query.Status, ignoreCase: true, out var status))
            instancesQuery = instancesQuery.Where(i => i.Status == status);

        var instances = await instancesQuery
            .OrderByDescending(i => i.Date)
            .Select(i => new ListChecklistInstancesResponseItem(
                i.Id, i.Template.Name, i.AssetId, i.Asset.Name, i.Asset.AreaId, i.Date, i.Status.ToString(), i.AssignedUserId, i.DurationSeconds))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListChecklistInstancesResponseItem>>(instances);
    }
}
