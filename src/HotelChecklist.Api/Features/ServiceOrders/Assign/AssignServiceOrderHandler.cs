using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ServiceOrders.Assign;

public sealed class AssignServiceOrderHandler(AppDbContext db) : ICommandHandler<AssignServiceOrderCommand, AssignServiceOrderResponse>
{
    public async Task<Result<AssignServiceOrderResponse>> Handle(AssignServiceOrderCommand command, CancellationToken cancellationToken)
    {
        var serviceOrder = await db.ServiceOrders.FirstOrDefaultAsync(so => so.Id == command.ServiceOrderId, cancellationToken);

        if (serviceOrder is null)
            return Result.Failure<AssignServiceOrderResponse>(Error.NotFound("ServiceOrders.NotFound", "Ordem de serviço no encontrada."));

        if (command.ActingUserIsExactlySupervisor)
        {
            var coversArea = await db.UserAreas.AnyAsync(
                ua => ua.UserId == command.ActingUserId && ua.AreaId == serviceOrder.AreaId, cancellationToken);

            if (!coversArea)
                return Result.Failure<AssignServiceOrderResponse>(
                    Error.Forbidden("ServiceOrders.AreaNotCovered", "No supervisás el área de esta orden de servicio."));
        }

        string? assignedUserName = null;

        if (command.UserId is not null)
        {
            assignedUserName = await db.Users
                .Where(u => u.Id == command.UserId && u.Active)
                .Select(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (assignedUserName is null)
                return Result.Failure<AssignServiceOrderResponse>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));
        }

        serviceOrder.AssignedUserId = command.UserId;

        var commentText = command.UserId is null ? "Ordem de serviço desdesignada." : $"Ordem de serviço designada a {assignedUserName}.";
        SystemServiceOrderCommentLog.Add(db, serviceOrder.Id, command.ActingUserId, commentText, DateTimeOffset.UtcNow);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AssignServiceOrderResponse(serviceOrder.Id, serviceOrder.AssignedUserId, assignedUserName));
    }
}
