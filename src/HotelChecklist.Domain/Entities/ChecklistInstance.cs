using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistInstance : IAuditable
{
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public Guid AssetId { get; set; }

    public DateOnly Date { get; set; }

    public ChecklistStatus Status { get; set; } = ChecklistStatus.Pending;

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public long? DurationSeconds { get; set; }

    public Guid? AssignedUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ChecklistTemplate Template { get; set; } = null!;

    public Asset Asset { get; set; } = null!;

    public User? AssignedUser { get; set; }

    public User? ApprovedByUser { get; set; }

    public ICollection<ChecklistTaskExecution> TaskExecutions { get; set; } = [];
}
