namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTaskComment
{
    public Guid Id { get; set; }

    public Guid ChecklistTaskExecutionId { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public Guid AuthorUserId { get; set; }

    public ChecklistTaskExecution ChecklistTaskExecution { get; set; } = null!;

    public User AuthorUser { get; set; } = null!;
}
