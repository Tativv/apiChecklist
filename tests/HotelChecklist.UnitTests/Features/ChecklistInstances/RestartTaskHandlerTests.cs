using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.RestartTask;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistInstances;

public class RestartTaskHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static (ChecklistInstance instance, ChecklistTaskExecution task) BuildCompletedInstanceWithTask(ChecklistStatus instanceStatus)
    {
        var instanceId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow.AddHours(-2);
        var completedAt = DateTimeOffset.UtcNow.AddHours(-1);
        var task = new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = instanceId,
            TaskId = Guid.NewGuid(),
            AssignedUserId = Guid.NewGuid(),
            Status = TaskExecutionStatus.Completed,
            StartedAt = startedAt,
            CompletedAt = completedAt,
            DurationSeconds = (long)(completedAt - startedAt).TotalSeconds,
            ExecutedByUserId = Guid.NewGuid()
        };
        var instance = new ChecklistInstance
        {
            Id = instanceId,
            TemplateId = Guid.NewGuid(),
            AssetId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = instanceStatus,
            StartedAt = startedAt,
            CompletedAt = instanceStatus == ChecklistStatus.Completed ? completedAt : null,
            DurationSeconds = instanceStatus == ChecklistStatus.Completed ? (long)(completedAt - startedAt).TotalSeconds : null,
            TaskExecutions = [task]
        };
        return (instance, task);
    }

    [Fact]
    public async Task Handle_CompletedTask_ShouldResetToPendingAndClearTiming()
    {
        await using var db = CreateDbContext();
        var (instance, task) = BuildCompletedInstanceWithTask(ChecklistStatus.Completed);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new RestartTaskHandler(db);
        var result = await handler.Handle(new RestartTaskCommand(instance.Id, task.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        var reloaded = await db.ChecklistTaskExecutions.FindAsync(task.Id);
        reloaded!.Status.Should().Be(TaskExecutionStatus.Pending);
        reloaded.StartedAt.Should().BeNull();
        reloaded.CompletedAt.Should().BeNull();
        reloaded.DurationSeconds.Should().BeNull();
        reloaded.ExecutedByUserId.Should().BeNull();
        reloaded.AssignedUserId.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_InstanceWasCompleted_ShouldRevertToInProgress()
    {
        await using var db = CreateDbContext();
        var (instance, task) = BuildCompletedInstanceWithTask(ChecklistStatus.Completed);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new RestartTaskHandler(db);
        await handler.Handle(new RestartTaskCommand(instance.Id, task.Id), CancellationToken.None);

        var reloadedInstance = await db.ChecklistInstances.FindAsync(instance.Id);
        reloadedInstance!.Status.Should().Be(ChecklistStatus.InProgress);
        reloadedInstance.CompletedAt.Should().BeNull();
        reloadedInstance.DurationSeconds.Should().BeNull();
    }

    [Fact]
    public async Task Handle_PendingTask_ShouldReturnConflict()
    {
        await using var db = CreateDbContext();
        var (instance, task) = BuildCompletedInstanceWithTask(ChecklistStatus.InProgress);
        task.Status = TaskExecutionStatus.Pending;
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new RestartTaskHandler(db);
        var result = await handler.Handle(new RestartTaskCommand(instance.Id, task.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
