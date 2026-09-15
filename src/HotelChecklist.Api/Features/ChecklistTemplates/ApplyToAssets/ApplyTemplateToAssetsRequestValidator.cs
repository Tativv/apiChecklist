using FluentValidation;

namespace HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;

public sealed class ApplyTemplateToAssetsRequestValidator : AbstractValidator<ApplyTemplateToAssetsRequest>
{
    public ApplyTemplateToAssetsRequestValidator()
    {
        RuleFor(r => r.AssetIds).NotEmpty();
    }
}
