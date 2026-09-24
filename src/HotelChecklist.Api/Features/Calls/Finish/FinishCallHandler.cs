using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.Calls.CallComments;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.Finish;

public sealed class FinishCallHandler(AppDbContext db) : ICommandHandler<FinishCallCommand, FinishCallResponse>
{
    public async Task<Result<FinishCallResponse>> Handle(FinishCallCommand command, CancellationToken cancellationToken)
    {
        var call = await db.Calls.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (call is null)
            return Result.Failure<FinishCallResponse>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        if (call.Status != CallStatus.InProgress)
            return Result.Failure<FinishCallResponse>(
                Error.Conflict("Calls.InvalidTransition", $"No se puede finalizar un chamado en estado {call.Status}."));

        if (!command.ActingUserIsSupervisorOrAbove && call.AssignedUserId != command.ActingUserId)
            return Result.Failure<FinishCallResponse>(
                Error.Forbidden("Calls.NotAssigned", "Solo la persona asignada o un supervisor pueden finalizar este chamado."));

        call.CompletedAt = DateTimeOffset.UtcNow;
        call.DurationSeconds = (long)(call.CompletedAt.Value - call.StartedAt!.Value).TotalSeconds;
        call.Status = CallStatus.Finished;

        SystemCallCommentLog.Add(db, call.Id, command.ActingUserId, "Chamado concluído.", call.CompletedAt.Value);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new FinishCallResponse(call.Id, call.Status.ToString(), call.CompletedAt, call.DurationSeconds));
    }
}
