using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.Finish;

public sealed class FinishServiceOrderHandler(AppDbContext db) : ICommandHandler<FinishServiceOrderCommand, FinishServiceOrderResponse>
{
    public async Task<Result<FinishServiceOrderResponse>> Handle(FinishServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var serviceOrder = await db.ServiceOrders.FirstOrDefaultAsync(so => so.Id == command.Id, cancellationToken);

        if (serviceOrder is null)
            return Result.Failure<FinishServiceOrderResponse>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        if (serviceOrder.Status != ServiceOrderStatus.InProgress)
            return Result.Failure<FinishServiceOrderResponse>(
                Error.Conflict("ServiceOrders.InvalidTransition", $"No se puede finalizar una orden de servicio en estado {serviceOrder.Status}."));

        serviceOrder.CompletedAt = DateTimeOffset.UtcNow;
        serviceOrder.DurationSeconds = (long)(serviceOrder.CompletedAt.Value - serviceOrder.StartedAt!.Value).TotalSeconds;
        serviceOrder.Status = ServiceOrderStatus.Finished;

        SystemServiceOrderCommentLog.Add(db, serviceOrder.Id, command.ActingUserId, "Ordem de serviço concluída.", serviceOrder.CompletedAt.Value);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new FinishServiceOrderResponse(serviceOrder.Id, serviceOrder.Status.ToString(), serviceOrder.CompletedAt, serviceOrder.DurationSeconds));
    }
}
