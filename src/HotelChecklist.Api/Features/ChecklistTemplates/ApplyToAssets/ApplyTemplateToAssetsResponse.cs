namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public sealed record ApplyTemplateToAssetsResponse(int Created, int Skipped, List<Guid> CreatedInstanceIds);
