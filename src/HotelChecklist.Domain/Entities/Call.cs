using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class Call : IAuditable
{
    public Guid Id { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid AreaId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string? Description { get; set; }

    public CallPriority Priority { get; set; }

    public CallStatus Status { get; set; } = CallStatus.Open;

    public Guid? AssignedUserId { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public long? DurationSeconds { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public Area Area { get; set; } = null!;

    public User? AssignedUser { get; set; }

    public ICollection<CallComment> Comments { get; set; } = [];
}
