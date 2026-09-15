namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTaskExecution
{
    public Guid Id { get; set; }

    public Guid ChecklistInstanceId { get; set; }

    public Guid TaskId { get; set; }

    public bool Completed { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public string? Comment { get; set; }

    public ChecklistInstance ChecklistInstance { get; set; } = null!;

    public ChecklistTask Task { get; set; } = null!;

    public ICollection<ChecklistTaskEvidence> Evidences { get; set; } = [];
}
