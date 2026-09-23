using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.List;

public sealed class ListCallsHandler(AppDbContext db) : IQueryHandler<ListCallsQuery, IReadOnlyList<ListCallsResponseItem>>
{
    public async Task<Result<IReadOnlyList<ListCallsResponseItem>>> Handle(ListCallsQuery query, CancellationToken cancellationToken)
    {
        var callsQuery = db.Calls.AsQueryable();

        if (query.AreaId is not null)
            callsQuery = callsQuery.Where(c => c.AreaId == query.AreaId);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<CallStatus>(query.Status, ignoreCase: true, out var status))
            callsQuery = callsQuery.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Priority) && Enum.TryParse<CallPriority>(query.Priority, ignoreCase: true, out var priority))
            callsQuery = callsQuery.Where(c => c.Priority == priority);

        if (query.AssignedUserId is not null)
            callsQuery = callsQuery.Where(c => c.AssignedUserId == query.AssignedUserId);

        if (query.RestrictToSupervisedAreas)
        {
            var supervisedAreaIds = await db.UserAreas
                .Where(ua => ua.UserId == query.ActingUserId)
                .Select(ua => ua.AreaId)
                .ToListAsync(cancellationToken);

            callsQuery = callsQuery.Where(c => supervisedAreaIds.Contains(c.AreaId));
        }

        if (query.RestrictToOwnOrUnassignedInAreas)
        {
            var myAreaIds = await db.UserAreas
                .Where(ua => ua.UserId == query.ActingUserId)
                .Select(ua => ua.AreaId)
                .ToListAsync(cancellationToken);

            callsQuery = callsQuery.Where(c =>
                c.AssignedUserId == query.ActingUserId
                || (c.AssignedUserId == null && myAreaIds.Contains(c.AreaId)));
        }

        // Priority se persiste como string (HasConversion<string>()), así que ordenar directo por
        // c.Priority ordena alfabéticamente la columna en la base ("Alta" < "Baixa" < "Media") en
        // vez de por urgencia real — se traduce a un CASE explícito para ordenar por urgencia.
        var calls = await callsQuery
            .OrderByDescending(c => c.Priority == CallPriority.Alta ? 2 : c.Priority == CallPriority.Media ? 1 : 0)
            .ThenBy(c => c.CreatedAtUtc)
            .Select(c => new ListCallsResponseItem(
                c.Id, c.AreaId, c.Area.Name, c.Subject, c.Priority.ToString(), c.Status.ToString(),
                c.CreatedByUserId, c.CreatedByUser.Name, c.AssignedUserId, c.AssignedUser != null ? c.AssignedUser.Name : null,
                c.StartedAt, c.CompletedAt, c.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ListCallsResponseItem>>(calls);
    }
}
