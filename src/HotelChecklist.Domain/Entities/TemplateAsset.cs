namespace HotelChecklist.Domain.Entities;

public sealed class TemplateAsset
{
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public Guid AssetId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public ChecklistTemplate Template { get; set; } = null!;

    public Asset Asset { get; set; } = null!;
}
