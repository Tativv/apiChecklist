using FluentValidation;

namespace HotelChecklist.Api.Features.Areas.Update;

public sealed class UpdateAreaRequestValidator : AbstractValidator<UpdateAreaRequest>
{
    public UpdateAreaRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
    }
}
