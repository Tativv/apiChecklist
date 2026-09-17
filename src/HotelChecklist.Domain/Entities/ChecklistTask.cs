namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTask
{
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Order { get; set; }

    public ChecklistTemplate Template { get; set; } = null!;

    public ICollection<ChecklistTaskExecution> Executions { get; set; } = [];
}
