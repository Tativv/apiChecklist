namespace HotelChecklist.Api.Features.ServiceOrders.Update;

public sealed record UpdateServiceOrderRequest(
    Guid AreaId,
    Guid AssetId,
    string Subject,
    string? Description,
    string Priority,
    DateTimeOffset DueAtUtc);
