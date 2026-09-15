using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Assets.Create;

public sealed class CreateAssetHandler(AppDbContext db) : ICommandHandler<CreateAssetCommand, CreateAssetResponse>
{
    public async Task<Result<CreateAssetResponse>> Handle(CreateAssetCommand command, CancellationToken cancellationToken)
    {
        var areaExists = await db.Areas.AnyAsync(a => a.Id == command.AreaId, cancellationToken);

        if (!areaExists)
            return Result.Failure<CreateAssetResponse>(Error.NotFound("Areas.NotFound", "Área no encontrada."));

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Type = command.Type,
            AreaId = command.AreaId,
            Active = true
        };

        db.Assets.Add(asset);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(asset.ToResponse());
    }
}
