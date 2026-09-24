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

    private static ChecklistInstance BuildInProgressInstance(Guid assignedUserId, bool allTasksDone)
    {
        var instance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            AssetId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = ChecklistStatus.InProgress,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-15),
            TaskExecutions =
            [
                new ChecklistTaskExecution
                {
                    Id = Guid.NewGuid(),
                    TaskId = Guid.NewGuid(),
                    Status = TaskExecutionStatus.Reviewed,
                    CompletedAt = DateTimeOffset.UtcNow,
                    AssignedUserId = assignedUserId
                },
                new ChecklistTaskExecution
                {
                    Id = Guid.NewGuid(),
                    TaskId = Guid.NewGuid(),
                    Status = allTasksDone ? TaskExecutionStatus.Completed : TaskExecutionStatus.InProgress,
                    CompletedAt = allTasksDone ? DateTimeOffset.UtcNow : null,
                    AssignedUserId = assignedUserId
                }
            ]
        };

        return instance;
    }

    [Fact]
    public async Task Handle_AllTasksCompletedOrReviewed_ShouldComputeDurationAndComplete()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        var instance = BuildInProgressInstance(userId, allTasksDone: true);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new FinishChecklistInstanceHandler(db);

        var result = await handler.Handle(new FinishChecklistInstanceCommand(instance.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        result.Value.Status.Should().Be(nameof(ChecklistStatus.Completed));
        result.Value.DurationSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_WithTasksNotYetDone_ShouldReturnValidationError()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        var instance = BuildInProgressInstance(userId, allTasksDone: false);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new FinishChecklistInstanceHandler(db);

        var result = await handler.Handle(new FinishChecklistInstanceCommand(instance.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }
}
