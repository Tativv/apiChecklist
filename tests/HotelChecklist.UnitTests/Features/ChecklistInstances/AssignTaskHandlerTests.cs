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
    public async Task Handle_LastTaskGetsAssigned_ShouldMoveInstanceToApproved()
    {
        await using var db = CreateDbContext();
        var instanceId = Guid.NewGuid();
        var collaborator = BuildActiveUser();
        var alreadyAssignedUser = BuildActiveUser();
        var instance = BuildInstance(instanceId, ChecklistStatus.Pending);
        var task1 = BuildTask(instanceId, alreadyAssignedUser.Id);
        var task2 = BuildTask(instanceId, null);
        db.Users.AddRange(collaborator, alreadyAssignedUser);
        db.ChecklistInstances.Add(instance);
        db.ChecklistTaskExecutions.AddRange(task1, task2);
        await db.SaveChangesAsync();

        var handler = new AssignTaskHandler(db);

        var result = await handler.Handle(
            new AssignTaskCommand(instanceId, task2.Id, collaborator.Id, Guid.NewGuid(), ActingUserIsExactlySupervisor: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        (await db.ChecklistInstances.FindAsync(instanceId))!.Status.Should().Be(ChecklistStatus.Approved);
    }

    [Fact]
    public async Task Handle_UnassignTaskOnApprovedInstance_ShouldRevertToPending()
    {
        await using var db = CreateDbContext();
        var instanceId = Guid.NewGuid();
        var assignedUser = BuildActiveUser();
        var instance = BuildInstance(instanceId, ChecklistStatus.Approved);
        var task1 = BuildTask(instanceId, assignedUser.Id);
        var task2 = BuildTask(instanceId, assignedUser.Id);
        db.Users.Add(assignedUser);
        db.ChecklistInstances.Add(instance);
        db.ChecklistTaskExecutions.AddRange(task1, task2);
        await db.SaveChangesAsync();

        var handler = new AssignTaskHandler(db);

        var result = await handler.Handle(
            new AssignTaskCommand(instanceId, task2.Id, null, Guid.NewGuid(), ActingUserIsExactlySupervisor: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        (await db.ChecklistInstances.FindAsync(instanceId))!.Status.Should().Be(ChecklistStatus.Pending);
    }

    [Fact]
    public async Task Handle_ReassignTaskOnInProgressInstance_ShouldNotChangeInstanceStatus()
    {
        await using var db = CreateDbContext();
        var instanceId = Guid.NewGuid();
        var collaborator = BuildActiveUser();
        var instance = BuildInstance(instanceId, ChecklistStatus.InProgress);
        var task1 = BuildTask(instanceId, collaborator.Id);
        db.Users.Add(collaborator);
        db.ChecklistInstances.Add(instance);
        db.ChecklistTaskExecutions.Add(task1);
        await db.SaveChangesAsync();

        var handler = new AssignTaskHandler(db);

        var result = await handler.Handle(
            new AssignTaskCommand(instanceId, task1.Id, null, Guid.NewGuid(), ActingUserIsExactlySupervisor: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        (await db.ChecklistInstances.FindAsync(instanceId))!.Status.Should().Be(ChecklistStatus.InProgress);
    }
}
