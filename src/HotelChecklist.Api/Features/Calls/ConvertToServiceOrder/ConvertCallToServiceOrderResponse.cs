namespace HotelChecklist.Api.Features.Calls.ConvertToServiceOrder;

public sealed record ConvertCallToServiceOrderResponse(
    Guid Id,
    Guid AreaId,
    string AreaName,
    Guid AssetId,
    string AssetName,
    Guid CallId,
    string Subject,
    string? Description,
    string Priority,
    string Status,
    DateTimeOffset DueAtUtc,
    bool Overdue,
    Guid CreatedByUserId,
    string CreatedByUserName,
    DateTimeOffset CreatedAtUtc);
