using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public sealed class ConvertCallToServiceOrderHandler(AppDbContext db) : ICommandHandler<ConvertCallToServiceOrderCommand, ConvertCallToServiceOrderResponse>
{
    public async Task<Result<ConvertCallToServiceOrderResponse>> Handle(ConvertCallToServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var call = await db.Calls.FirstOrDefaultAsync(c => c.Id == command.CallId, cancellationToken);

        if (call is null)
            return Result.Failure<ConvertCallToServiceOrderResponse>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        var alreadyConverted = await db.ServiceOrders.AnyAsync(so => so.CallId == command.CallId, cancellationToken);

        if (alreadyConverted)
            return Result.Failure<ConvertCallToServiceOrderResponse>(
                Error.Conflict("ServiceOrders.AlreadyConverted", "Este chamado ya fue convertido en una orden de serviço."));

        var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == command.AssetId, cancellationToken);

        if (asset is null)
            return Result.Failure<ConvertCallToServiceOrderResponse>(Error.NotFound("Assets.NotFound", "Ativo no encontrado."));

        if (asset.AreaId != call.AreaId)
            return Result.Failure<ConvertCallToServiceOrderResponse>(
                Error.Validation("ServiceOrders.AssetAreaMismatch", "O ativo não pertence à área do chamado."));

        var area = await db.Areas.FirstAsync(a => a.Id == call.AreaId, cancellationToken);
        var createdByUser = await db.Users.FirstAsync(u => u.Id == command.ActingUserId, cancellationToken);

        var serviceOrder = new ServiceOrder
        {
            Id = Guid.NewGuid(),
            AreaId = call.AreaId,
            AssetId = command.AssetId,
            CallId = call.Id,
            Subject = command.Subject ?? call.Subject,
            Description = command.Description ?? call.Description,
            Priority = Enum.Parse<ServiceOrderPriority>(command.Priority ?? call.Priority.ToString(), ignoreCase: true),
            Status = ServiceOrderStatus.Open,
            DueAtUtc = command.DueAtUtc,
            CreatedByUserId = command.ActingUserId
        };

        db.ServiceOrders.Add(serviceOrder);
        await db.SaveChangesAsync(cancellationToken);

        var overdue = serviceOrder.DueAtUtc < DateTimeOffset.UtcNow && serviceOrder.Status != ServiceOrderStatus.Finished;

        return Result.Success(new ConvertCallToServiceOrderResponse(
            serviceOrder.Id, serviceOrder.AreaId, area.Name, serviceOrder.AssetId, asset.Name, call.Id,
            serviceOrder.Subject, serviceOrder.Description, serviceOrder.Priority.ToString(), serviceOrder.Status.ToString(),
            serviceOrder.DueAtUtc, overdue, serviceOrder.CreatedByUserId, createdByUser.Name, serviceOrder.CreatedAtUtc));
    }
}
