namespace HotelChecklist.Api.Features.ChecklistInstances.GetMyAssignedTasks;

public sealed record MyAssignedTaskItem(
    Guid InstanceId,
    string TemplateName,
    Guid AssetId,
    string AssetName,
    DateOnly Date,
    Guid TaskExecutionId,
    string TaskName,
    DateTimeOffset? ScheduledForUtc,
    string Status);
