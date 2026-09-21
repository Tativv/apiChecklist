namespace HotelChecklist.Api.Features.ChecklistInstances.GetById;

public sealed record GetChecklistInstanceByIdResponse(
    Guid Id,
    Guid TemplateId,
    string TemplateName,
    Guid AssetId,
    string AssetName,
    DateOnly Date,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    long? DurationSeconds,
    Guid? AssignedUserId,
    Guid? ApprovedByUserId,
    DateTimeOffset? ApprovedAt,
    List<TaskExecutionResponseItem> TaskExecutions);

public sealed record TaskExecutionResponseItem(
    Guid Id,
    Guid TaskId,
    string TaskName,
    int Order,
    string Status,
    DateTimeOffset? ScheduledForUtc,
    DateTimeOffset? ExecutedAtUtc,
    string? Comment,
    Guid? CompletedByUserId,
    int EvidenceCount);
