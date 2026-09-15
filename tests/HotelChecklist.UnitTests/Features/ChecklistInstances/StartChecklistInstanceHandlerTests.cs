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
        AssignedUserId = assignedUserId
    };

    [Fact]
    public async Task Handle_UnassignedPendingInstance_ShouldSelfAssignAndStart()
    {
        await using var db = CreateDbContext();
        var instance = BuildInstance();
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var actingUserId = Guid.NewGuid();
        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(new StartChecklistInstanceCommand(instance.Id, actingUserId, ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(nameof(ChecklistStatus.InProgress));
        result.Value.AssignedUserId.Should().Be(actingUserId);
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
    public async Task Handle_AssignedToAnotherOperator_ShouldReturnForbidden()
    {
        await using var db = CreateDbContext();
        var assignedUserId = Guid.NewGuid();
        var instance = BuildInstance(assignedUserId: assignedUserId);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(
            new StartChecklistInstanceCommand(instance.Id, Guid.NewGuid(), ActingUserIsSupervisorOrAbove: false), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public async Task Handle_AssignedToAnotherOperator_ButActingUserIsSupervisor_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var assignedUserId = Guid.NewGuid();
        var instance = BuildInstance(assignedUserId: assignedUserId);
        db.ChecklistInstances.Add(instance);
        await db.SaveChangesAsync();

        var handler = new StartChecklistInstanceHandler(db);

        var result = await handler.Handle(
            new StartChecklistInstanceCommand(instance.Id, Guid.NewGuid(), ActingUserIsSupervisorOrAbove: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AssignedUserId.Should().Be(assignedUserId);
    }
}
