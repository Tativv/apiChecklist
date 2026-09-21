using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.Finish;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistInstances;

public class FinishChecklistInstanceHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static ChecklistInstance BuildInProgressInstance(Guid assignedUserId, bool allTasksCompleted)
    {
        var instance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            AssetId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = ChecklistStatus.InProgress,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-15),
            AssignedUserId = assignedUserId,
            TaskExecutions =
            [
                new ChecklistTaskExecution { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), Status = TaskExecutionStatus.Completed, ExecutedAtUtc = DateTimeOffset.UtcNow },
                new ChecklistTaskExecution
                {
                    Id = Guid.NewGuid(),
                    TaskId = Guid.NewGuid(),
                    Status = allTasksCompleted ? TaskExecutionStatus.Completed : TaskExecutionStatus.Pending,
                    ExecutedAtUtc = allTasksCompleted ? DateTimeOffset.UtcNow : null
                }
            ]
        };

        return instance;
    }

    [Fact]
    public async Task Handle_AllTasksCompleted_ShouldComputeDurationAndComplete()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        var instance = BuildInProgressInstance(userId, allTasksCompleted: true);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new FinishChecklistInstanceHandler(db);

        var result = await handler.Handle(new FinishChecklistInstanceCommand(instance.Id, userId, ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(nameof(ChecklistStatus.Completed));
        result.Value.DurationSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_WithPendingTasks_ShouldReturnValidationError()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        var instance = BuildInProgressInstance(userId, allTasksCompleted: false);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new FinishChecklistInstanceHandler(db);

        var result = await handler.Handle(new FinishChecklistInstanceCommand(instance.Id, userId, ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_ByUnassignedNonSupervisorUser_ShouldReturnForbidden()
    {
        await using var db = CreateDbContext();
        var assignedUserId = Guid.NewGuid();
        var instance = BuildInProgressInstance(assignedUserId, allTasksCompleted: true);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new FinishChecklistInstanceHandler(db);

        var result = await handler.Handle(
            new FinishChecklistInstanceCommand(instance.Id, Guid.NewGuid(), ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
