namespace HotelChecklist.Api.Features.ServiceOrders.GetById;

public sealed record GetServiceOrderByIdResponse(
    Guid Id,
    Guid AreaId,
    string AreaName,
    Guid AssetId,
    string AssetName,
    Guid? CallId,
    string Subject,
    string? Description,
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
    long? DurationSeconds,
    DateTimeOffset CreatedAtUtc,
    int CommentCount);
