using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.GetById;

public sealed class GetServiceOrderByIdHandler(AppDbContext db) : IQueryHandler<GetServiceOrderByIdQuery, GetServiceOrderByIdResponse>
{
    public async Task<Result<GetServiceOrderByIdResponse>> Handle(GetServiceOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var response = await db.ServiceOrders
            .Where(so => so.Id == query.Id)
            .Select(so => new GetServiceOrderByIdResponse(
                so.Id, so.AreaId, so.Area.Name, so.AssetId, so.Asset.Name, so.CallId, so.Subject, so.Description,
                so.Priority.ToString(), so.Status.ToString(), so.DueAtUtc, so.DueAtUtc < now && so.Status != ServiceOrderStatus.Finished,
                so.CreatedByUserId, so.CreatedByUser.Name, so.AssignedUserId, so.AssignedUser != null ? so.AssignedUser.Name : null,
                so.StartedAt, so.CompletedAt, so.DurationSeconds, so.CreatedAtUtc, so.Comments.Count))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return Result.Failure<GetServiceOrderByIdResponse>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        return Result.Success(response);
    }
}
