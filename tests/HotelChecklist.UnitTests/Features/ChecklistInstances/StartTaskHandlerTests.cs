using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.StartTask;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistInstances;

public class StartTaskHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static (ChecklistInstance instance, ChecklistTaskExecution task) BuildInstanceWithTask(Guid? assignedUserId)
    {
        var instanceId = Guid.NewGuid();
        var task = new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = instanceId,
            TaskId = Guid.NewGuid(),
            Status = TaskExecutionStatus.Pending,
            AssignedUserId = assignedUserId
        };
        var instance = new ChecklistInstance
        {
            Id = instanceId,
            TemplateId = Guid.NewGuid(),
            AssetId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = ChecklistStatus.InProgress,
            StartedAt = DateTimeOffset.UtcNow,
            TaskExecutions = [task]
        };
        return (instance, task);
    }

    [Fact]
    public async Task Handle_AssignedUserStartsOwnTask_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        var (instance, task) = BuildInstanceWithTask(userId);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartTaskHandler(db);
        var result = await handler.Handle(new StartTaskCommand(instance.Id, task.Id, userId, ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        result.Value.Status.Should().Be(nameof(TaskExecutionStatus.InProgress));
        (await db.ChecklistTaskExecutions.FindAsync(task.Id))!.StartedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_OtherColaboradorNotAssigned_ShouldBeForbidden()
    {
        await using var db = CreateDbContext();
        var (instance, task) = BuildInstanceWithTask(Guid.NewGuid());
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartTaskHandler(db);
        var result = await handler.Handle(
            new StartTaskCommand(instance.Id, task.Id, Guid.NewGuid(), ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ChecklistTaskExecutions.NotAssigned");
    }

    [Fact]
    public async Task Handle_SupervisorCanStartEvenIfNotAssigned_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var (instance, task) = BuildInstanceWithTask(Guid.NewGuid());
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartTaskHandler(db);
        var result = await handler.Handle(
            new StartTaskCommand(instance.Id, task.Id, Guid.NewGuid(), ActingUserIsSupervisorOrAbove: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
    }

    [Fact]
    public async Task Handle_TaskAlreadyInProgress_ShouldReturnConflict()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        var (instance, task) = BuildInstanceWithTask(userId);
        task.Status = TaskExecutionStatus.InProgress;
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartTaskHandler(db);
        var result = await handler.Handle(new StartTaskCommand(instance.Id, task.Id, userId, ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
