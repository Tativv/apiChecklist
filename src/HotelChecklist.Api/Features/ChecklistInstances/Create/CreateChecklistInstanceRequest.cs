namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public sealed record CreateChecklistInstanceRequest(Guid TemplateId, Guid AssetId, DateOnly Date);
