using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.Start;

public sealed class StartServiceOrderHandler(AppDbContext db) : ICommandHandler<StartServiceOrderCommand, StartServiceOrderResponse>
{
    public async Task<Result<StartServiceOrderResponse>> Handle(StartServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var serviceOrder = await db.ServiceOrders.FirstOrDefaultAsync(so => so.Id == command.Id, cancellationToken);

        if (serviceOrder is null)
            return Result.Failure<StartServiceOrderResponse>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        if (serviceOrder.Status != ServiceOrderStatus.Open)
            return Result.Failure<StartServiceOrderResponse>(
                Error.Conflict("ServiceOrders.InvalidTransition", $"No se puede iniciar una orden de servicio en estado {serviceOrder.Status}."));

        serviceOrder.Status = ServiceOrderStatus.InProgress;
        serviceOrder.StartedAt = DateTimeOffset.UtcNow;

        SystemServiceOrderCommentLog.Add(db, serviceOrder.Id, command.ActingUserId, "Ordem de serviço iniciada.", serviceOrder.StartedAt.Value);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new StartServiceOrderResponse(serviceOrder.Id, serviceOrder.Status.ToString(), serviceOrder.StartedAt));
    }
}
