using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public sealed class ConvertCallToServiceOrderRequestValidator : AbstractValidator<ConvertCallToServiceOrderRequest>
{
    public ConvertCallToServiceOrderRequestValidator()
    {
        RuleFor(r => r.AssetId).NotEmpty();
        RuleFor(r => r.Subject).MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(2000);

        RuleFor(r => r.Priority)
            .Must(p => Enum.TryParse<ServiceOrderPriority>(p, ignoreCase: true, out _))
            .When(r => r.Priority is not null)
            .WithMessage($"Priority must be one of: {string.Join(", ", Enum.GetNames<ServiceOrderPriority>())}.");

        RuleFor(r => r.DueAtUtc)
            .NotEqual(default(DateTimeOffset))
            .WithMessage("DueAtUtc é obrigatório.");
    }
}
