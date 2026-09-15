using FluentValidation;

namespace HotelChecklist.Api.Features.Assets.Create;

public sealed class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Type).NotEmpty().MaximumLength(100);
        RuleFor(r => r.AreaId).NotEmpty();
    }
}
