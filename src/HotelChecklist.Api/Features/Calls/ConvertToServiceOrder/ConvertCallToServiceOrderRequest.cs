namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public sealed record ConvertCallToServiceOrderRequest(
    Guid AssetId,
    DateTimeOffset DueAtUtc,
    string? Priority,
    string? Subject,
    string? Description);
