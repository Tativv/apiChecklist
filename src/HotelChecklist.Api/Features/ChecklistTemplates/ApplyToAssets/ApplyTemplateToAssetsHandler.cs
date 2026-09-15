using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public sealed class ApplyTemplateToAssetsHandler(AppDbContext db, ChecklistInstanceCreationService creationService)
    : ICommandHandler<ApplyTemplateToAssetsCommand, ApplyTemplateToAssetsResponse>
{
    public async Task<Result<ApplyTemplateToAssetsResponse>> Handle(ApplyTemplateToAssetsCommand command, CancellationToken cancellationToken)
    {
        var templateExists = await db.ChecklistTemplates.AnyAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (!templateExists)
            return Result.Failure<ApplyTemplateToAssetsResponse>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        var date = command.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var createdIds = new List<Guid>();
        var skipped = 0;

        foreach (var assetId in command.AssetIds)
        {
            var result = await creationService.CreateAsync(command.TemplateId, assetId, date, command.AssignedUserId, cancellationToken);

            if (result.IsSuccess)
                createdIds.Add(result.Value.Id);
            else
                skipped++;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new ApplyTemplateToAssetsResponse(createdIds.Count, skipped, createdIds));
    }
}
