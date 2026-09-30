using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.Update;

public sealed class UpdateCallHandler(AppDbContext db) : ICommandHandler<UpdateCallCommand, UpdateCallResponse>
{
    public async Task<Result<UpdateCallResponse>> Handle(UpdateCallCommand command, CancellationToken cancellationToken)
    {
        var call = await db.Calls.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (call is null)
            return Result.Failure<UpdateCallResponse>(Error.NotFound("Calls.NotFound", "Chamado no encontrado."));

        var area = await db.Areas.FirstOrDefaultAsync(a => a.Id == command.AreaId, cancellationToken);

        if (area is null)
            return Result.Failure<UpdateCallResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        call.AreaId = command.AreaId;
        call.Subject = command.Subject;
        call.Description = command.Description;
        call.Priority = Enum.Parse<CallPriority>(command.Priority, ignoreCase: true);

        await db.SaveChangesAsync(cancellationToken);

        var createdByUser = await db.Users.Where(u => u.Id == call.CreatedByUserId).Select(u => u.Name).FirstAsync(cancellationToken);
        var assignedUserName = call.AssignedUserId is null
            ? null
            : await db.Users.Where(u => u.Id == call.AssignedUserId).Select(u => u.Name).FirstOrDefaultAsync(cancellationToken);
        var commentCount = await db.CallComments.CountAsync(cc => cc.CallId == call.Id, cancellationToken);

        return Result.Success(new UpdateCallResponse(
            call.Id, call.AreaId, area.Name, call.Subject, call.Description, call.Priority.ToString(), call.Status.ToString(),
            call.CreatedByUserId, createdByUser, call.AssignedUserId, assignedUserName,
            call.StartedAt, call.CompletedAt, call.DurationSeconds, call.CreatedAtUtc, commentCount));
    }
}
