using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.Calls.CallComments;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.Assign;

public sealed class AssignCallHandler(AppDbContext db) : ICommandHandler<AssignCallCommand, AssignCallResponse>
{
    public async Task<Result<AssignCallResponse>> Handle(AssignCallCommand command, CancellationToken cancellationToken)
    {
        var call = await db.Calls.FirstOrDefaultAsync(c => c.Id == command.CallId, cancellationToken);

        if (call is null)
            return Result.Failure<AssignCallResponse>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        if (command.ActingUserIsExactlyColaborador)
        {
            if (command.UserId != command.ActingUserId)
                return Result.Failure<AssignCallResponse>(
                    Error.Forbidden("Calls.CannotAssignOthers", "Un colaborador solo puede auto-asignarse el chamado."));

            if (call.AssignedUserId is not null)
                return Result.Failure<AssignCallResponse>(
                    Error.Conflict("Calls.AlreadyAssigned", "Este chamado ya fue asignado a otra persona."));

            var coversArea = await db.UserAreas.AnyAsync(
                ua => ua.UserId == command.ActingUserId && ua.AreaId == call.AreaId, cancellationToken);

            if (!coversArea)
                return Result.Failure<AssignCallResponse>(
                    Error.Forbidden("Calls.AreaNotCovered", "No cubrís el área de este chamado."));
        }
        else if (command.ActingUserIsExactlySupervisor)
        {
            var coversArea = await db.UserAreas.AnyAsync(
                ua => ua.UserId == command.ActingUserId && ua.AreaId == call.AreaId, cancellationToken);

            if (!coversArea)
                return Result.Failure<AssignCallResponse>(
                    Error.Forbidden("Calls.AreaNotCovered", "No supervisás el área de este chamado."));
        }

        string? assignedUserName = null;

        if (command.UserId is not null)
        {
            assignedUserName = await db.Users
                .Where(u => u.Id == command.UserId && u.Active)
                .Select(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (assignedUserName is null)
                return Result.Failure<AssignCallResponse>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));
        }

        call.AssignedUserId = command.UserId;

        var commentText = command.UserId is null ? "Chamado desdesignado." : $"Chamado designado a {assignedUserName}.";
        SystemCallCommentLog.Add(db, call.Id, command.ActingUserId, commentText, DateTimeOffset.UtcNow);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AssignCallResponse(call.Id, call.AssignedUserId, assignedUserName));
    }
}
