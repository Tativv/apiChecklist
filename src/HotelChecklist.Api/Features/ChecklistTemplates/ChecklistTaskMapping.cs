using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates;

public static class ChecklistTaskMapping
{
    public static ChecklistTask ToTask(this ChecklistTaskRequest request) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Description = request.Description,
        Order = request.Order,
        EstimatedDurationMinutes = request.EstimatedDurationMinutes,
        ExecutionMode = Enum.Parse<TaskExecutionMode>(request.ExecutionMode, ignoreCase: true),
        TaskSchedules = request.Schedules
            .Select(s => new TaskSchedule { Id = Guid.NewGuid(), Schedule = s.ToSchedule() })
            .ToList()
    };

    public static ChecklistTaskResponseItem ToResponseItem(this ChecklistTask task) => new(
        task.Id,
        task.Name,
        task.Description,
        task.Order,
        task.EstimatedDurationMinutes,
        task.ExecutionMode.ToString(),
        task.TaskSchedules.OrderBy(ts => ts.Schedule.ExecutionOrder).Select(ts => ts.Schedule.ToResponseItem()).ToList());
}
