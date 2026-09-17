using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;

public sealed class ConfigureTemplateAssetsRequestValidator : AbstractValidator<ConfigureTemplateAssetsRequest>
{
    public ConfigureTemplateAssetsRequestValidator()
    {
        RuleFor(r => r.AssetIds).NotNull();
    }
}
