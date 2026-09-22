namespace HotelChecklist.Domain.Entities;

public sealed class UserArea
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid AreaId { get; set; }

    public User User { get; set; } = null!;

    public Area Area { get; set; } = null!;
}
