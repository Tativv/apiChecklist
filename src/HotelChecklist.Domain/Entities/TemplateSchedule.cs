namespace HotelChecklist.Domain.Entities;

public sealed class TemplateSchedule
{
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public Guid ScheduleId { get; set; }

    public ChecklistTemplate Template { get; set; } = null!;

    public Schedule Schedule { get; set; } = null!;
}
