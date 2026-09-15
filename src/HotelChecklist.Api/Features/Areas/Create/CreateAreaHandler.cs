using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Areas.Create;

public sealed class CreateAreaHandler(AppDbContext db) : ICommandHandler<CreateAreaCommand, CreateAreaResponse>
{
    public async Task<Result<CreateAreaResponse>> Handle(CreateAreaCommand command, CancellationToken cancellationToken)
    {
        var nameInUse = await db.Areas.AnyAsync(a => a.Name == command.Name, cancellationToken);

        if (nameInUse)
            return Result.Failure<CreateAreaResponse>(Error.Conflict("Areas.NameInUse", "Ya existe un área con ese nombre."));

        var area = new Area { Id = Guid.NewGuid(), Name = command.Name };

        db.Areas.Add(area);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(area.ToResponse());
    }
}
