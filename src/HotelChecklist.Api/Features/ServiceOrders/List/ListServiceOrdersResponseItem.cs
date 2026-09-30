namespace HotelChecklist.Api.Features.ServiceOrders.List;

public sealed record ListServiceOrdersResponseItem(
    Guid Id,
    Guid AreaId,
    string AreaName,
    Guid AssetId,
    string AssetName,
    Guid? CallId,
    string Subject,
    string Priority,
    string Status,
    DateTimeOffset DueAtUtc,
    bool Overdue,
    Guid CreatedByUserId,
    string CreatedByUserName,
    Guid? AssignedUserId,
    string? AssignedUserName,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAtUtc,
    int CommentCount);
