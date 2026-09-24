namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTaskComment
{
    public Guid Id { get; set; }

    public Guid ChecklistTaskExecutionId { get; set; }

    /// <summary>Nullable: un comentario puede ser solo un archivo adjunto, sin texto.</summary>
    public string? Text { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Guid AuthorUserId { get; set; }

    public string? FilePath { get; set; }

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public long? FileSizeBytes { get; set; }

    public ChecklistTaskExecution ChecklistTaskExecution { get; set; } = null!;

    public User AuthorUser { get; set; } = null!;
}
