using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public sealed record ConvertCallToServiceOrderCommand(
    Guid CallId,
    Guid AssetId,
    DateTimeOffset DueAtUtc,
    string? Priority,
    string? Subject,
    string? Description,
    Guid ActingUserId) : ICommand<ConvertCallToServiceOrderResponse>;
