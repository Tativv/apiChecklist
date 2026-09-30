using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ServiceOrder : IAuditable
{
    public Guid Id { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid AreaId { get; set; }

    public Guid AssetId { get; set; }

    public Guid? CallId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ServiceOrderPriority Priority { get; set; }

    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Open;

    public DateTimeOffset DueAtUtc { get; set; }

    public Guid? AssignedUserId { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public long? DurationSeconds { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public Area Area { get; set; } = null!;

    public Asset Asset { get; set; } = null!;

    public Call? Call { get; set; }

    public User? AssignedUser { get; set; }

    public ICollection<ServiceOrderComment> Comments { get; set; } = [];
}
