using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public sealed class ConfigureTemplateAssetsHandler(AppDbContext db)
    : ICommandHandler<ConfigureTemplateAssetsCommand, ConfigureTemplateAssetsResponse>
{
    public async Task<Result<ConfigureTemplateAssetsResponse>> Handle(ConfigureTemplateAssetsCommand command, CancellationToken cancellationToken)
    {
        var templateExists = await db.ChecklistTemplates.AnyAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (!templateExists)
            return Result.Failure<ConfigureTemplateAssetsResponse>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        var requestedAssetIds = command.AssetIds.Distinct().ToList();

        var validAssetCount = await db.Assets.CountAsync(a => requestedAssetIds.Contains(a.Id), cancellationToken);

        if (validAssetCount != requestedAssetIds.Count)
            return Result.Failure<ConfigureTemplateAssetsResponse>(Error.NotFound("Assets.NotFound", "Uno o más activos no existen."));

        var current = await db.TemplateAssets
            .Where(ta => ta.TemplateId == command.TemplateId)
            .ToListAsync(cancellationToken);

        var currentAssetIds = current.Select(ta => ta.AssetId).ToHashSet();
        var requestedSet = requestedAssetIds.ToHashSet();

        var toRemove = current.Where(ta => !requestedSet.Contains(ta.AssetId)).ToList();
        var toAdd = requestedAssetIds.Where(id => !currentAssetIds.Contains(id)).ToList();

        db.TemplateAssets.RemoveRange(toRemove);
        db.TemplateAssets.AddRange(toAdd.Select(assetId => new TemplateAsset
        {
            Id = Guid.NewGuid(),
            TemplateId = command.TemplateId,
            AssetId = assetId,
            CreatedAtUtc = DateTimeOffset.UtcNow
        }));

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new ConfigureTemplateAssetsResponse(requestedAssetIds.Count));
    }
}
