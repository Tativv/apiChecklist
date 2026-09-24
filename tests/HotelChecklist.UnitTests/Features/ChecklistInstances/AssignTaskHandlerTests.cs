using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistInstances.AssignTask;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistInstances;

public class AssignTaskHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static User BuildActiveUser() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Colaborador Test",
        Email = $"{Guid.NewGuid()}@test.local",
        Role = UserRole.Colaborador,
        PasswordHash = "hash",
        Active = true
    };

    private static ChecklistTaskExecution BuildTask(Guid instanceId, Guid? assignedUserId) => new()
    {
        Id = Guid.NewGuid(),
        ChecklistInstanceId = instanceId,
        TaskId = Guid.NewGuid(),
        AssignedUserId = assignedUserId
    };

    private static ChecklistInstance BuildInstance(Guid instanceId, ChecklistStatus status) => new()
    {
        Id = instanceId,
        TemplateId = Guid.NewGuid(),
        AssetId = Guid.NewGuid(),
        Date = DateOnly.FromDateTime(DateTime.UtcNow),
        Status = status
    };

    [Fact]
    public async Task Handle_FirstAssignmentOnPendingInstance_ShouldMoveToInProgress()
    {
        await using var db = CreateDbContext();
        var instanceId = Guid.NewGuid();
        var collaborator = BuildActiveUser();
        var instance = BuildInstance(instanceId, ChecklistStatus.Pending);
        var task1 = BuildTask(instanceId, null);
        db.Users.Add(collaborator);
        db.ChecklistInstances.Add(instance);
        db.ChecklistTaskExecutions.Add(task1);
        await db.SaveChangesAsync();

        var handler = new AssignTaskHandler(db);

        var result = await handler.Handle(
            new AssignTaskCommand(instanceId, task1.Id, collaborator.Id, EstimatedDurationMinutes: 20, Guid.NewGuid(), ActingUserIsExactlySupervisor: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        var updatedInstance = await db.ChecklistInstances.FindAsync(instanceId);
        updatedInstance!.Status.Should().Be(ChecklistStatus.InProgress);
        updatedInstance.StartedAt.Should().NotBeNull();
        (await db.ChecklistTaskExecutions.FindAsync(task1.Id))!.EstimatedDurationMinutes.Should().Be(20);
    }

    [Fact]
    public async Task Handle_SecondAssignmentOnInProgressInstance_ShouldNotChangeInstanceStatus()
    {
        await using var db = CreateDbContext();
        var instanceId = Guid.NewGuid();
        var collaborator = BuildActiveUser();
        var instance = BuildInstance(instanceId, ChecklistStatus.InProgress);
        instance.StartedAt = DateTimeOffset.UtcNow.AddMinutes(-5);
        var task1 = BuildTask(instanceId, null);
        db.Users.Add(collaborator);
        db.ChecklistInstances.Add(instance);
        db.ChecklistTaskExecutions.Add(task1);
        await db.SaveChangesAsync();
        var originalStartedAt = instance.StartedAt;

        var handler = new AssignTaskHandler(db);

        var result = await handler.Handle(
            new AssignTaskCommand(instanceId, task1.Id, collaborator.Id, null, Guid.NewGuid(), ActingUserIsExactlySupervisor: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        var updatedInstance = await db.ChecklistInstances.FindAsync(instanceId);
        updatedInstance!.Status.Should().Be(ChecklistStatus.InProgress);
        updatedInstance.StartedAt.Should().Be(originalStartedAt);
    }

    [Fact]
    public async Task Handle_UnassignTaskOnPendingInstance_ShouldNotStartIt()
    {
        await using var db = CreateDbContext();
        var instanceId = Guid.NewGuid();
        var instance = BuildInstance(instanceId, ChecklistStatus.Pending);
        var task1 = BuildTask(instanceId, Guid.NewGuid());
        db.ChecklistInstances.Add(instance);
        db.ChecklistTaskExecutions.Add(task1);
        await db.SaveChangesAsync();

        var handler = new AssignTaskHandler(db);

        var result = await handler.Handle(
            new AssignTaskCommand(instanceId, task1.Id, null, null, Guid.NewGuid(), ActingUserIsExactlySupervisor: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        (await db.ChecklistInstances.FindAsync(instanceId))!.Status.Should().Be(ChecklistStatus.Pending);
    }
}
