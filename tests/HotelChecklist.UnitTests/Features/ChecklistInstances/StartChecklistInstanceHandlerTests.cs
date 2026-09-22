using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.Start;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistInstances;

public class StartChecklistInstanceHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static ChecklistInstance BuildInstance(ChecklistStatus status = ChecklistStatus.Pending, Guid? assignedUserId = null) => new()
    {
        Id = Guid.NewGuid(),
        TemplateId = Guid.NewGuid(),
        AssetId = Guid.NewGuid(),
        Date = DateOnly.FromDateTime(DateTime.UtcNow),
        Status = status,
        TaskExecutions = assignedUserId is null
            ? []
            : [new ChecklistTaskExecution { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), AssignedUserId = assignedUserId }]
    };

    [Fact]
    public async Task Handle_ActingUserHasAssignedTask_ShouldStart()
    {
        await using var db = CreateDbContext();
        var actingUserId = Guid.NewGuid();
        var instance = BuildInstance(assignedUserId: actingUserId);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(new StartChecklistInstanceCommand(instance.Id, actingUserId, ActingUserIsManagerOrAbove: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(nameof(ChecklistStatus.InProgress));
    }

    [Fact]
    public async Task Handle_AlreadyInProgress_ShouldReturnConflict()
    {
        await using var db = CreateDbContext();
        var instance = BuildInstance(ChecklistStatus.InProgress);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(new StartChecklistInstanceCommand(instance.Id, Guid.NewGuid(), false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_NoAssignedTaskAndNotManager_ShouldReturnForbidden()
    {
        await using var db = CreateDbContext();
        var instance = BuildInstance(assignedUserId: Guid.NewGuid());
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(
            new StartChecklistInstanceCommand(instance.Id, Guid.NewGuid(), ActingUserIsManagerOrAbove: false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public async Task Handle_NoAssignedTaskButActingUserIsManager_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var instance = BuildInstance(assignedUserId: Guid.NewGuid());
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(
            new StartChecklistInstanceCommand(instance.Id, Guid.NewGuid(), ActingUserIsManagerOrAbove: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotAllTasksAssigned_ShouldReturnValidationError_EvenForManager()
    {
        await using var db = CreateDbContext();
        var actingUserId = Guid.NewGuid();
        var instance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            AssetId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = ChecklistStatus.Pending,
            TaskExecutions =
            [
                new ChecklistTaskExecution { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), AssignedUserId = actingUserId },
                new ChecklistTaskExecution { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), AssignedUserId = null }
            ]
        };
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(
            new StartChecklistInstanceCommand(instance.Id, actingUserId, ActingUserIsManagerOrAbove: true), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }
}
