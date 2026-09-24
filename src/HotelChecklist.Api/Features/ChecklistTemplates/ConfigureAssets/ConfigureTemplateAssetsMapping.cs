using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public static class ConfigureTemplateAssetsMapping
{
    public static ConfigureTemplateAssetsCommand ToCommand(this ConfigureTemplateAssetsRequest request, Guid templateId, UserRole actingUserRole) =>
        new(templateId, request.AssetIds, actingUserRole);
}
