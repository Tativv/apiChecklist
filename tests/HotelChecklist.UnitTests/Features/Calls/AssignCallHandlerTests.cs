using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.Calls.Assign;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.Calls;

public class AssignCallHandlerTests
{
    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static User BuildActiveUser(UserRole role = UserRole.Colaborador) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Usuario Test",
        Email = $"{Guid.NewGuid()}@test.local",
        Role = role,
        PasswordHash = "hash",
        Active = true
    };

    private static Call BuildCall(Guid areaId, Guid createdByUserId, Guid? assignedUserId = null) => new()
    {
        Id = Guid.NewGuid(),
        AreaId = areaId,
        CreatedByUserId = createdByUserId,
        Subject = "Test",
        Priority = CallPriority.Media,
        Status = CallStatus.Open,
        AssignedUserId = assignedUserId
    };

    [Fact]
    public async Task Handle_ColaboradorSelfAssignsUnassignedCallInOwnArea_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var colaborador = BuildActiveUser();
        var call = BuildCall(area.Id, creator.Id);
        db.Areas.Add(area);
        db.Users.AddRange(creator, colaborador);
        db.Calls.Add(call);
        db.UserAreas.Add(new UserArea { Id = Guid.NewGuid(), UserId = colaborador.Id, AreaId = area.Id });
        await db.SaveChangesAsync();

        var handler = new AssignCallHandler(db);
        var result = await handler.Handle(
            new AssignCallCommand(call.Id, colaborador.Id, colaborador.Id, ActingUserIsExactlySupervisor: false, ActingUserIsExactlyColaborador: true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        (await db.Calls.FindAsync(call.Id))!.AssignedUserId.Should().Be(colaborador.Id);
    }

    [Fact]
    public async Task Handle_ColaboradorTriesToAssignSomeoneElse_ShouldBeForbidden()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var colaborador = BuildActiveUser();
        var otherUser = BuildActiveUser();
        var call = BuildCall(area.Id, creator.Id);
        db.Areas.Add(area);
        db.Users.AddRange(creator, colaborador, otherUser);
        db.Calls.Add(call);
        db.UserAreas.Add(new UserArea { Id = Guid.NewGuid(), UserId = colaborador.Id, AreaId = area.Id });
        await db.SaveChangesAsync();

        var handler = new AssignCallHandler(db);
        var result = await handler.Handle(
            new AssignCallCommand(call.Id, otherUser.Id, colaborador.Id, ActingUserIsExactlySupervisor: false, ActingUserIsExactlyColaborador: true),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Calls.CannotAssignOthers");
    }

    [Fact]
    public async Task Handle_ColaboradorSelfAssignsAlreadyAssignedCall_ShouldConflict()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var colaborador = BuildActiveUser();
        var someoneElse = BuildActiveUser();
        var call = BuildCall(area.Id, creator.Id, someoneElse.Id);
        db.Areas.Add(area);
        db.Users.AddRange(creator, colaborador, someoneElse);
        db.Calls.Add(call);
        db.UserAreas.Add(new UserArea { Id = Guid.NewGuid(), UserId = colaborador.Id, AreaId = area.Id });
        await db.SaveChangesAsync();

        var handler = new AssignCallHandler(db);
        var result = await handler.Handle(
            new AssignCallCommand(call.Id, colaborador.Id, colaborador.Id, ActingUserIsExactlySupervisor: false, ActingUserIsExactlyColaborador: true),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Calls.AlreadyAssigned");
    }

    [Fact]
    public async Task Handle_ColaboradorOutsideAreaSelfAssigns_ShouldBeForbidden()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var colaborador = BuildActiveUser();
        var call = BuildCall(area.Id, creator.Id);
        db.Areas.Add(area);
        db.Users.AddRange(creator, colaborador);
        db.Calls.Add(call);
        await db.SaveChangesAsync();

        var handler = new AssignCallHandler(db);
        var result = await handler.Handle(
            new AssignCallCommand(call.Id, colaborador.Id, colaborador.Id, ActingUserIsExactlySupervisor: false, ActingUserIsExactlyColaborador: true),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Calls.AreaNotCovered");
    }

    [Fact]
    public async Task Handle_SupervisorOutsideAreaAssignsToOther_ShouldBeForbidden()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var supervisor = BuildActiveUser(UserRole.Supervisor);
        var target = BuildActiveUser();
        var call = BuildCall(area.Id, creator.Id);
        db.Areas.Add(area);
        db.Users.AddRange(creator, supervisor, target);
        db.Calls.Add(call);
        await db.SaveChangesAsync();

        var handler = new AssignCallHandler(db);
        var result = await handler.Handle(
            new AssignCallCommand(call.Id, target.Id, supervisor.Id, ActingUserIsExactlySupervisor: true, ActingUserIsExactlyColaborador: false),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Calls.AreaNotCovered");
    }

    [Fact]
    public async Task Handle_ManagerAssignsToAnyoneAnyArea_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var manager = BuildActiveUser(UserRole.Gerencia);
        var target = BuildActiveUser();
        var call = BuildCall(area.Id, manager.Id);
        db.Areas.Add(area);
        db.Users.AddRange(manager, target);
        db.Calls.Add(call);
        await db.SaveChangesAsync();

        var handler = new AssignCallHandler(db);
        var result = await handler.Handle(
            new AssignCallCommand(call.Id, target.Id, manager.Id, ActingUserIsExactlySupervisor: false, ActingUserIsExactlyColaborador: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        (await db.Calls.FindAsync(call.Id))!.AssignedUserId.Should().Be(target.Id);
    }
}
