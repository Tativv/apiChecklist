namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public sealed record CreateChecklistInstanceResponse(
    Guid Id,
    Guid TemplateId,
    Guid AssetId,
    DateOnly Date,
    string Status,
    Guid? AssignedUserId);
