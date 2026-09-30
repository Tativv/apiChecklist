using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.Calls.Update;

public sealed class UpdateCallRequestValidator : AbstractValidator<UpdateCallRequest>
{
    public UpdateCallRequestValidator()
    {
        RuleFor(r => r.AreaId).NotEmpty();
        RuleFor(r => r.Subject).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(2000);

        RuleFor(r => r.Priority)
            .NotEmpty()
            .Must(p => Enum.TryParse<CallPriority>(p, ignoreCase: true, out _))
            .WithMessage($"Priority must be one of: {string.Join(", ", Enum.GetNames<CallPriority>())}.");
    }
}
