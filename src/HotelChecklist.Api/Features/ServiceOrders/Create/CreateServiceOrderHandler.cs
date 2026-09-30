using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.Create;

public sealed class CreateServiceOrderHandler(AppDbContext db) : ICommandHandler<CreateServiceOrderCommand, CreateServiceOrderResponse>
{
    public async Task<Result<CreateServiceOrderResponse>> Handle(CreateServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var area = await db.Areas.FirstOrDefaultAsync(a => a.Id == command.AreaId, cancellationToken);

        if (area is null)
            return Result.Failure<CreateServiceOrderResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == command.AssetId, cancellationToken);

        if (asset is null)
            return Result.Failure<CreateServiceOrderResponse>(Error.NotFound("Assets.NotFound", "Ativo no encontrado."));

        if (asset.AreaId != command.AreaId)
            return Result.Failure<CreateServiceOrderResponse>(
                Error.Validation("ServiceOrders.AssetAreaMismatch", "O ativo não pertence à área informada."));

        var createdByUser = await db.Users.FirstAsync(u => u.Id == command.CreatedByUserId, cancellationToken);

        var serviceOrder = new ServiceOrder
        {
            Id = Guid.NewGuid(),
            AreaId = command.AreaId,
            AssetId = command.AssetId,
            Subject = command.Subject,
            Description = command.Description,
            Priority = Enum.Parse<ServiceOrderPriority>(command.Priority, ignoreCase: true),
            Status = ServiceOrderStatus.Open,
            DueAtUtc = command.DueAtUtc,
            CreatedByUserId = command.CreatedByUserId
        };

        db.ServiceOrders.Add(serviceOrder);
        await db.SaveChangesAsync(cancellationToken);

        var overdue = serviceOrder.DueAtUtc < DateTimeOffset.UtcNow && serviceOrder.Status != ServiceOrderStatus.Finished;

        return Result.Success(new CreateServiceOrderResponse(
            serviceOrder.Id, serviceOrder.AreaId, area.Name, serviceOrder.AssetId, asset.Name, serviceOrder.CallId,
            serviceOrder.Subject, serviceOrder.Description, serviceOrder.Priority.ToString(), serviceOrder.Status.ToString(),
            serviceOrder.DueAtUtc, overdue, serviceOrder.CreatedByUserId, createdByUser.Name, serviceOrder.AssignedUserId, null,
            serviceOrder.StartedAt, serviceOrder.CompletedAt, serviceOrder.DurationSeconds, serviceOrder.CreatedAtUtc));
    }
}
