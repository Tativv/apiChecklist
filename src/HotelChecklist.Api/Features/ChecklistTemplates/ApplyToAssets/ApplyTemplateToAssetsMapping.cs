namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public static class ApplyTemplateToAssetsMapping
{
    public static ApplyTemplateToAssetsCommand ToCommand(this ApplyTemplateToAssetsRequest request, Guid templateId) =>
        new(templateId, request.AssetIds, request.Date, request.AssignedUserId);
}
