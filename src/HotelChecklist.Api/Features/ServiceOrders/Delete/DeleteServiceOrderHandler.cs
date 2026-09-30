using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;

namespace HotelChecklist.Api.Features.ServiceOrders.Delete;

public sealed class DeleteServiceOrderHandler(AppDbContext db) : ICommandHandler<DeleteServiceOrderCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var serviceOrder = await db.ServiceOrders.FindAsync([command.Id], cancellationToken);

        if (serviceOrder is null)
            return Result.Failure<Unit>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        db.ServiceOrders.Remove(serviceOrder);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
