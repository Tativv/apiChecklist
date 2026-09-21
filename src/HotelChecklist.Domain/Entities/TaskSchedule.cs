namespace HotelChecklist.Domain.Entities;

public sealed class TaskSchedule
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public Guid ScheduleId { get; set; }

    public ChecklistTask Task { get; set; } = null!;

    public Schedule Schedule { get; set; } = null!;
}
