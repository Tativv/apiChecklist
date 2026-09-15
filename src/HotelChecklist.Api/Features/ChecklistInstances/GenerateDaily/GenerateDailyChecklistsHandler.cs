using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateDaily;

public sealed class GenerateDailyChecklistsHandler(AppDbContext db, ChecklistInstanceCreationService creationService)
    : ICommandHandler<GenerateDailyChecklistsCommand, GenerateDailyChecklistsResponse>
{
    public async Task<Result<GenerateDailyChecklistsResponse>> Handle(GenerateDailyChecklistsCommand command, CancellationToken cancellationToken)
    {
        var date = command.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var dailyTemplates = await db.ChecklistTemplates
            .Where(t => t.RecurrenceType == ChecklistRecurrenceType.Daily)
            .ToListAsync(cancellationToken);

        var created = 0;
        var skipped = 0;

        foreach (var template in dailyTemplates)
        {
            var assetIds = await db.Assets
                .Where(a => a.AreaId == template.AreaId && a.Active)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            foreach (var assetId in assetIds)
            {
                var result = await creationService.CreateAsync(template.Id, assetId, date, assignedUserId: null, cancellationToken);

                if (result.IsSuccess)
                    created++;
                else
                    skipped++;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new GenerateDailyChecklistsResponse(date, created, skipped));
    }
}
