using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.Update;

public sealed class UpdateServiceOrderHandler(AppDbContext db) : ICommandHandler<UpdateServiceOrderCommand, UpdateServiceOrderResponse>
{
    public async Task<Result<UpdateServiceOrderResponse>> Handle(UpdateServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var serviceOrder = await db.ServiceOrders.FirstOrDefaultAsync(so => so.Id == command.Id, cancellationToken);

        if (serviceOrder is null)
            return Result.Failure<UpdateServiceOrderResponse>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        var area = await db.Areas.FirstOrDefaultAsync(a => a.Id == command.AreaId, cancellationToken);

        if (area is null)
            return Result.Failure<UpdateServiceOrderResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == command.AssetId, cancellationToken);

        if (asset is null)
            return Result.Failure<UpdateServiceOrderResponse>(Error.NotFound("Assets.NotFound", "Ativo no encontrado."));

        if (asset.AreaId != command.AreaId)
            return Result.Failure<UpdateServiceOrderResponse>(
                Error.Validation("ServiceOrders.AssetAreaMismatch", "O ativo não pertence à área informada."));

        serviceOrder.AreaId = command.AreaId;
        serviceOrder.AssetId = command.AssetId;
        serviceOrder.Subject = command.Subject;
        serviceOrder.Description = command.Description;
        serviceOrder.Priority = Enum.Parse<ServiceOrderPriority>(command.Priority, ignoreCase: true);
        serviceOrder.DueAtUtc = command.DueAtUtc;

        await db.SaveChangesAsync(cancellationToken);

        var createdByUser = await db.Users.Where(u => u.Id == serviceOrder.CreatedByUserId).Select(u => u.Name).FirstAsync(cancellationToken);
        var assignedUserName = serviceOrder.AssignedUserId is null
            ? null
            : await db.Users.Where(u => u.Id == serviceOrder.AssignedUserId).Select(u => u.Name).FirstOrDefaultAsync(cancellationToken);

        var overdue = serviceOrder.DueAtUtc < DateTimeOffset.UtcNow && serviceOrder.Status != ServiceOrderStatus.Finished;

        return Result.Success(new UpdateServiceOrderResponse(
            serviceOrder.Id, serviceOrder.AreaId, area.Name, serviceOrder.AssetId, asset.Name, serviceOrder.CallId,
            serviceOrder.Subject, serviceOrder.Description, serviceOrder.Priority.ToString(), serviceOrder.Status.ToString(),
            serviceOrder.DueAtUtc, overdue, serviceOrder.CreatedByUserId, createdByUser, serviceOrder.AssignedUserId, assignedUserName,
            serviceOrder.StartedAt, serviceOrder.CompletedAt, serviceOrder.DurationSeconds, serviceOrder.CreatedAtUtc));
    }
}
