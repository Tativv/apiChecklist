using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTaskExecution
{
    public Guid Id { get; set; }

    public Guid ChecklistInstanceId { get; set; }

    public Guid TaskId { get; set; }

    public Guid? ScheduleId { get; set; }

    public DateTimeOffset? ScheduledForUtc { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public long? DurationSeconds { get; set; }

    public TaskExecutionStatus Status { get; set; } = TaskExecutionStatus.Pending;

    public string? Comment { get; set; }

    /// <summary>Colaborador responsable de ejecutar esta tarea.</summary>
    public Guid? AssignedUserId { get; set; }

    /// <summary>Supervisor+ que realizó la asignación.</summary>
    public Guid? CreatedByUserId { get; set; }

    /// <summary>Colaborador que efectivamente marcó la tarea como ejecutada.</summary>
    public Guid? ExecutedByUserId { get; set; }

    /// <summary>Supervisor+ que aprobó esta tarea puntual.</summary>
    public Guid? ApprovedByUserId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public ChecklistInstance ChecklistInstance { get; set; } = null!;

    public ChecklistTask Task { get; set; } = null!;

    public Schedule? Schedule { get; set; }

    public User? AssignedUser { get; set; }

    public User? CreatedByUser { get; set; }

    public User? ExecutedByUser { get; set; }

    public User? ApprovedByUser { get; set; }

    public ICollection<ChecklistTaskEvidence> Evidences { get; set; } = [];

    public ICollection<ChecklistTaskComment> Comments { get; set; } = [];
}
