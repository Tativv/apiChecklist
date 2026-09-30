using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.List;

public sealed class ListServiceOrdersHandler(AppDbContext db) : IQueryHandler<ListServiceOrdersQuery, IReadOnlyList<ListServiceOrdersResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListServiceOrdersResponseItem>>> Handle(ListServiceOrdersQuery query, CancellationToken cancellationToken)
    {
        var serviceOrdersQuery = db.ServiceOrders.AsQueryable();

        if (query.AreaId is not null)
            serviceOrdersQuery = serviceOrdersQuery.Where(so => so.AreaId == query.AreaId);

        if (query.AssetId is not null)
            serviceOrdersQuery = serviceOrdersQuery.Where(so => so.AssetId == query.AssetId);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<ServiceOrderStatus>(query.Status, ignoreCase: true, out var status))
            serviceOrdersQuery = serviceOrdersQuery.Where(so => so.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Priority) && Enum.TryParse<ServiceOrderPriority>(query.Priority, ignoreCase: true, out var priority))
            serviceOrdersQuery = serviceOrdersQuery.Where(so => so.Priority == priority);

        if (query.AssignedUserId is not null)
            serviceOrdersQuery = serviceOrdersQuery.Where(so => so.AssignedUserId == query.AssignedUserId);

        if (query.RestrictToSupervisedAreas)
        {
            var supervisedAreaIds = await db.UserAreas
                .Where(ua => ua.UserId == query.ActingUserId)
                .Select(ua => ua.AreaId)
                .ToListAsync(cancellationToken);

            serviceOrdersQuery = serviceOrdersQuery.Where(so => supervisedAreaIds.Contains(so.AreaId));
        }

        if (query.RestrictToOwnOrUnassignedInAreas)
        {
            var myAreaIds = await db.UserAreas
                .Where(ua => ua.UserId == query.ActingUserId)
                .Select(ua => ua.AreaId)
                .ToListAsync(cancellationToken);

            serviceOrdersQuery = serviceOrdersQuery.Where(so =>
                so.AssignedUserId == query.ActingUserId
                || (so.AssignedUserId == null && myAreaIds.Contains(so.AreaId)));
        }

        var now = DateTimeOffset.UtcNow;

        // Priority se persiste como string (HasConversion<string>()), así que ordenar directo por
        // so.Priority ordena alfabéticamente la columna en vez de por urgencia real — se traduce a
        // un CASE explícito. A diferencia de Calls, se desempata por DueAtUtc (más urgente primero)
        // en vez de CreatedAtUtc, porque acá la fecha de vencimiento es el dato operativo relevante.
        var serviceOrders = await serviceOrdersQuery
            .OrderByDescending(so => so.Priority == ServiceOrderPriority.Alta ? 2 : so.Priority == ServiceOrderPriority.Media ? 1 : 0)
            .ThenBy(so => so.DueAtUtc)
            .Select(so => new ListServiceOrdersResponseItem(
                so.Id, so.AreaId, so.Area.Name, so.AssetId, so.Asset.Name, so.CallId, so.Subject, so.Priority.ToString(), so.Status.ToString(),
                so.DueAtUtc, so.DueAtUtc < now && so.Status != ServiceOrderStatus.Finished,
                so.CreatedByUserId, so.CreatedByUser.Name, so.AssignedUserId, so.AssignedUser != null ? so.AssignedUser.Name : null,
                so.StartedAt, so.CompletedAt, so.CreatedAtUtc, so.Comments.Count))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListServiceOrdersResponseItem>>(serviceOrders);
    }
}
