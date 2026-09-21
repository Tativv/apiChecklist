namespace HotelChecklist.Api.Features.ChecklistInstances.GetUpcomingOccurrences;

public sealed record UpcomingOccurrenceItem(
    Guid TemplateId,
    string TemplateName,
    Guid AssetId,
    string AssetName,
    DateOnly Date,
    string ScheduledTime,
    bool AlreadyGenerated);
