using FluentValidation;

namespace HotelChecklist.Api.Features.Assets.Update;

public sealed class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
{
    public UpdateAssetRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Type).NotEmpty().MaximumLength(100);
        RuleFor(r => r.AreaId).NotEmpty();
    }
}
