using FluentValidation;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ServiceOrders.Create;

public sealed class CreateServiceOrderRequestValidator : AbstractValidator<CreateServiceOrderRequest>
{
    public CreateServiceOrderRequestValidator()
    {
        RuleFor(r => r.AreaId).NotEmpty();
        RuleFor(r => r.AssetId).NotEmpty();
        RuleFor(r => r.Subject).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(2000);

        RuleFor(r => r.Priority)
            .NotEmpty()
            .Must(p => Enum.TryParse<ServiceOrderPriority>(p, ignoreCase: true, out _))
            .WithMessage($"Priority must be one of: {string.Join(", ", Enum.GetNames<ServiceOrderPriority>())}.");

        RuleFor(r => r.DueAtUtc)
            .NotEqual(default(DateTimeOffset))
            .WithMessage("DueAtUtc é obrigatório.");
    }
}
