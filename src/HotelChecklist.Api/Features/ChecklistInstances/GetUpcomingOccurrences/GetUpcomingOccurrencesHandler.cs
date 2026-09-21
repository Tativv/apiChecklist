using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistTemplates;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetUpcomingOccurrences;

public sealed class GetUpcomingOccurrencesHandler(AppDbContext db, ScheduleEvaluationService evaluationService)
    : IQueryHandler<GetUpcomingOccurrencesQuery, IReadOnlyList<UpcomingOccurrenceItem>>
{
    public async Task<Result<IReadOnlyList<UpcomingOccurrenceItem>>> Handle(GetUpcomingOccurrencesQuery query, CancellationToken cancellationToken)
    {
        if (query.To < query.From)
            return Result.Failure<IReadOnlyList<UpcomingOccurrenceItem>>(
                Error.Validation("ChecklistInstances.InvalidRange", "'to' must not be before 'from'."));

        if (query.To.DayNumber - query.From.DayNumber > 366)
            return Result.Failure<IReadOnlyList<UpcomingOccurrenceItem>>(
                Error.Validation("ChecklistInstances.RangeTooLarge", "Date range must not exceed 366 days."));

        var templates = await db.ChecklistTemplates
            .Include(t => t.TemplateSchedules).ThenInclude(ts => ts.Schedule)
            .Include(t => t.TemplateAssets).ThenInclude(ta => ta.Asset)
            .ToListAsync(cancellationToken);

        var existingInstanceKeys = await db.ChecklistInstances
            .Where(i => i.Date >= query.From && i.Date <= query.To)
            .Select(i => new { i.TemplateId, i.AssetId, i.Date })
            .ToListAsync(cancellationToken);

        var existingSet = existingInstanceKeys.Select(i => (i.TemplateId, i.AssetId, i.Date)).ToHashSet();

        var items = new List<UpcomingOccurrenceItem>();

        for (var date = query.From; date <= query.To; date = date.AddDays(1))
        {
            foreach (var template in templates)
            {
                var matchingSchedule = template.TemplateSchedules
                    .Where(ts => evaluationService.ShouldExecute(ts.Schedule, date))
                    .OrderBy(ts => ts.Schedule.ExecutionOrder)
                    .Select(ts => ts.Schedule)
                    .FirstOrDefault();

                if (matchingSchedule is null)
                    continue;

                foreach (var templateAsset in template.TemplateAssets)
                {
                    items.Add(new UpcomingOccurrenceItem(
                        template.Id,
                        template.Name,
                        templateAsset.AssetId,
                        templateAsset.Asset.Name,
                        date,
                        ScheduleMapping.FormatTimeOfDay(matchingSchedule.TimeOfDay),
                        existingSet.Contains((template.Id, templateAsset.AssetId, date))));
                }
            }
        }

        return Result.Success<IReadOnlyList<UpcomingOccurrenceItem>>(
            items.OrderBy(i => i.Date).ThenBy(i => i.ScheduledTime).ToList());
    }
}
