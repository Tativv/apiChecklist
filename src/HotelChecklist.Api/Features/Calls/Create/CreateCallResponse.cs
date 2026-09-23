namespace HotelChecklist.Api.Features.Calls.Create;

public sealed record CreateCallResponse(
    Guid Id,
    Guid AreaId,
    string AreaName,
    string Subject,
    string? Description,
    string Priority,
    string Status,
    Guid CreatedByUserId,
    string CreatedByUserName,
    Guid? AssignedUserId,
    string? AssignedUserName,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    long? DurationSeconds,
    DateTimeOffset CreatedAtUtc);
