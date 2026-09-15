namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTaskEvidence
{
    public Guid Id { get; set; }

    public Guid ChecklistTaskExecutionId { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DateTimeOffset UploadedAt { get; set; }

    public Guid UploadedByUserId { get; set; }

    public ChecklistTaskExecution ChecklistTaskExecution { get; set; } = null!;

    public User UploadedByUser { get; set; } = null!;
}
