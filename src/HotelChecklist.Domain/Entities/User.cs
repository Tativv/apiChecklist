using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class User : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ICollection<ChecklistTaskEvidence> UploadedEvidences { get; set; } = [];

    public ICollection<UserArea> UserAreas { get; set; } = [];
}
