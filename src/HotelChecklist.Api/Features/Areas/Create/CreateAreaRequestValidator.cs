using FluentValidation;

namespace HotelChecklist.Api.Features.Areas.Create;

public sealed class CreateAreaRequestValidator : AbstractValidator<CreateAreaRequest>
{
    public CreateAreaRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
    }
}
