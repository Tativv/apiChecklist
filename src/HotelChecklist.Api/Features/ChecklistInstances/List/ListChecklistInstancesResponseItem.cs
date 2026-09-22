namespace HotelChecklist.Api.Features.ChecklistInstances.List;

public sealed record ListChecklistInstancesResponseItem(
    Guid Id,
    string TemplateName,
    Guid AssetId,
    string AssetName,
    Guid AreaId,
    DateOnly Date,
    string Status,
    long? DurationSeconds);
