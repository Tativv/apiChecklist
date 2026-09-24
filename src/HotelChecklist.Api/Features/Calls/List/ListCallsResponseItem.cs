namespace HotelChecklist.Api.Features.Calls.List;

public sealed record ListCallsResponseItem(
    Guid Id,
    Guid AreaId,
    string AreaName,
    string Subject,
    string Priority,
    string Status,
    Guid CreatedByUserId,
    string CreatedByUserName,
    Guid? AssignedUserId,
    string? AssignedUserName,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAtUtc,
    int CommentCount);
