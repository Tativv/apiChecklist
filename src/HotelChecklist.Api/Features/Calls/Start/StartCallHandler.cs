using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.Start;

public sealed class StartCallHandler(AppDbContext db) : ICommandHandler<StartCallCommand, StartCallResponse>
{
    public async Task<Result<StartCallResponse>> Handle(StartCallCommand command, CancellationToken cancellationToken)
    {
        var call = await db.Calls.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (call is null)
            return Result.Failure<StartCallResponse>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        if (call.Status != CallStatus.Open)
            return Result.Failure<StartCallResponse>(
                Error.Conflict("Calls.InvalidTransition", $"No se puede iniciar un chamado en estado {call.Status}."));

        if (!command.ActingUserIsSupervisorOrAbove && call.AssignedUserId != command.ActingUserId)
            return Result.Failure<StartCallResponse>(
                Error.Forbidden("Calls.NotAssigned", "Solo la persona asignada o un supervisor pueden iniciar este chamado."));

        call.Status = CallStatus.InProgress;
        call.StartedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new StartCallResponse(call.Id, call.Status.ToString(), call.StartedAt));
    }
}
