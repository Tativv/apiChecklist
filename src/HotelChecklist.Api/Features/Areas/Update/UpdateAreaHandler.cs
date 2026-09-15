using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Areas.Update;

public sealed class UpdateAreaHandler(AppDbContext db) : ICommandHandler<UpdateAreaCommand, UpdateAreaResponse>
{
    public async Task<Result<UpdateAreaResponse>> Handle(UpdateAreaCommand command, CancellationToken cancellationToken)
    {
        var area = await db.Areas.FindAsync([command.Id], cancellationToken);

        if (area is null)
            return Result.Failure<UpdateAreaResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var nameInUse = await db.Areas.AnyAsync(a => a.Name == command.Name && a.Id != command.Id, cancellationToken);

        if (nameInUse)
            return Result.Failure<UpdateAreaResponse>(Error.Conflict("Areas.NameInUse", "Ya existe un área con ese nombre."));

        area.Name = command.Name;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(area.ToResponse());
    }
}
