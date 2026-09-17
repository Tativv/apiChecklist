namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public static class ConfigureTemplateAssetsMapping
{
    public static ConfigureTemplateAssetsCommand ToCommand(this ConfigureTemplateAssetsRequest request, Guid templateId) =>
        new(templateId, request.AssetIds);
}
