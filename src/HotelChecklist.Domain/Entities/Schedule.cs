using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class Schedule
{
    public Guid Id { get; set; }

    public ScheduleFrequencyType FrequencyType { get; set; }

    public int IntervalValue { get; set; } = 1;

    public DayOfWeek? WeekDay { get; set; }

    public int? DayOfMonth { get; set; }

    public TimeOnly TimeOfDay { get; set; }

    public int ExecutionOrder { get; set; }

    public bool Active { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }
}
