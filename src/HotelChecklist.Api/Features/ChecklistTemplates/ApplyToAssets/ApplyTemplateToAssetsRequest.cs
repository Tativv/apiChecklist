namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public sealed record ApplyTemplateToAssetsRequest(List<Guid> AssetIds, DateOnly? Date, Guid? AssignedUserId);
