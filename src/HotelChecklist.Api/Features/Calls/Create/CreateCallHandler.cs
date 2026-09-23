using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Calls.Create;

public sealed class CreateCallHandler(AppDbContext db) : ICommandHandler<CreateCallCommand, CreateCallResponse>
{
    public async Task<Result<CreateCallResponse>> Handle(CreateCallCommand command, CancellationToken cancellationToken)
    {
        var area = await db.Areas.FirstOrDefaultAsync(a => a.Id == command.AreaId, cancellationToken);

        if (area is null)
            return Result.Failure<CreateCallResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var createdByUser = await db.Users.FirstAsync(u => u.Id == command.CreatedByUserId, cancellationToken);

        var call = new Call
        {
            Id = Guid.NewGuid(),
            AreaId = command.AreaId,
            Subject = command.Subject,
            Description = command.Description,
            Priority = Enum.Parse<CallPriority>(command.Priority, ignoreCase: true),
            Status = CallStatus.Open,
            CreatedByUserId = command.CreatedByUserId
        };

        db.Calls.Add(call);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateCallResponse(
            call.Id, call.AreaId, area.Name, call.Subject, call.Description, call.Priority.ToString(), call.Status.ToString(),
            call.CreatedByUserId, createdByUser.Name, call.AssignedUserId, null, call.StartedAt, call.CompletedAt, call.DurationSeconds, call.CreatedAtUtc));
    }
}
