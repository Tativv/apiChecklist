using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTaskExecution
{
    public Guid Id { get; set; }

    public Guid ChecklistInstanceId { get; set; }

    public Guid TaskId { get; set; }

    public Guid? ScheduleId { get; set; }

    public DateTimeOffset? ScheduledForUtc { get; set; }

    public DateTimeOffset? ExecutedAtUtc { get; set; }

    public TaskExecutionStatus Status { get; set; } = TaskExecutionStatus.Pending;

    public string? Comment { get; set; }

    public Guid? CompletedByUserId { get; set; }

    public ChecklistInstance ChecklistInstance { get; set; } = null!;

    public ChecklistTask Task { get; set; } = null!;

    public Schedule? Schedule { get; set; }

    public User? CompletedByUser { get; set; }

    public ICollection<ChecklistTaskEvidence> Evidences { get; set; } = [];
}
