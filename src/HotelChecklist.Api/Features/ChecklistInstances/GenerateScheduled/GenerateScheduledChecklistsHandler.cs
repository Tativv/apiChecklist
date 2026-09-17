using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateScheduled;

public sealed class GenerateScheduledChecklistsHandler(AppDbContext db, ChecklistInstanceCreationService creationService)
    : ICommandHandler<GenerateScheduledChecklistsCommand, GenerateScheduledChecklistsResponse>
{
    public async Task<Result<GenerateScheduledChecklistsResponse>> Handle(GenerateScheduledChecklistsCommand command, CancellationToken cancellationToken)
    {
        var date = command.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var templates = await db.ChecklistTemplates
            .Include(t => t.TemplateAssets)
            .ToListAsync(cancellationToken);

        var created = 0;
        var skipped = 0;

        foreach (var template in templates)
        {
            if (!ChecklistTemplateRecurrenceEvaluator.ShouldGenerate(template, date))
                continue;

            foreach (var templateAsset in template.TemplateAssets)
            {
                var result = await creationService.CreateAsync(template.Id, templateAsset.AssetId, date, assignedUserId: null, cancellationToken);

                if (result.IsSuccess)
                    created++;
                else
                    skipped++;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new GenerateScheduledChecklistsResponse(date, created, skipped));
    }
}
