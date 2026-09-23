using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.Calls.List;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.Calls;

public class ListCallsHandlerTests
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

    [Fact]
    public async Task Handle_ColaboradorScope_ShouldSeeOwnAssignedAndUnassignedInArea_NotOthersAssigned()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var colaborador = BuildActiveUser();
        var otherColaborador = BuildActiveUser();
        db.Areas.Add(area);
        db.Users.AddRange(creator, colaborador, otherColaborador);
        db.UserAreas.Add(new UserArea { Id = Guid.NewGuid(), UserId = colaborador.Id, AreaId = area.Id });

        var assignedToMe = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "Mío", Priority = CallPriority.Media, Status = CallStatus.Open, AssignedUserId = colaborador.Id };
        var unassignedInMyArea = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "Libre", Priority = CallPriority.Baixa, Status = CallStatus.Open };
        var assignedToOther = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "De otro", Priority = CallPriority.Alta, Status = CallStatus.Open, AssignedUserId = otherColaborador.Id };
        db.Calls.AddRange(assignedToMe, unassignedInMyArea, assignedToOther);
        await db.SaveChangesAsync();

        var handler = new ListCallsHandler(db);
        var result = await handler.Handle(
            new ListCallsQuery(null, null, null, null, colaborador.Id, RestrictToSupervisedAreas: false, RestrictToOwnOrUnassignedInAreas: true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(c => c.Id).Should().BeEquivalentTo([assignedToMe.Id, unassignedInMyArea.Id]);
    }

    [Fact]
    public async Task Handle_SupervisorScope_ShouldSeeAllCallsInAreaRegardlessOfAssignment()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var otherArea = new Area { Id = Guid.NewGuid(), Name = "Otra Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        var supervisor = BuildActiveUser(UserRole.Supervisor);
        var colaborador = BuildActiveUser();
        db.Areas.AddRange(area, otherArea);
        db.Users.AddRange(creator, supervisor, colaborador);
        db.UserAreas.Add(new UserArea { Id = Guid.NewGuid(), UserId = supervisor.Id, AreaId = area.Id });

        var inArea = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "En área", Priority = CallPriority.Media, Status = CallStatus.Open, AssignedUserId = colaborador.Id };
        var inOtherArea = new Call { Id = Guid.NewGuid(), AreaId = otherArea.Id, CreatedByUserId = creator.Id, Subject = "Otra área", Priority = CallPriority.Media, Status = CallStatus.Open };
        db.Calls.AddRange(inArea, inOtherArea);
        await db.SaveChangesAsync();

        var handler = new ListCallsHandler(db);
        var result = await handler.Handle(
            new ListCallsQuery(null, null, null, null, supervisor.Id, RestrictToSupervisedAreas: true, RestrictToOwnOrUnassignedInAreas: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(c => c.Id).Should().BeEquivalentTo([inArea.Id]);
    }

    [Fact]
    public async Task Handle_OrdersByPriorityDescendingThenOldestFirst()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var creator = BuildActiveUser(UserRole.Directoria);
        db.Areas.Add(area);
        db.Users.Add(creator);

        var baixa = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "Baixa", Priority = CallPriority.Baixa, Status = CallStatus.Open };
        var alta = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "Alta", Priority = CallPriority.Alta, Status = CallStatus.Open };
        var media = new Call { Id = Guid.NewGuid(), AreaId = area.Id, CreatedByUserId = creator.Id, Subject = "Média", Priority = CallPriority.Media, Status = CallStatus.Open };
        db.Calls.AddRange(baixa, alta, media);
        await db.SaveChangesAsync();

        var handler = new ListCallsHandler(db);
        var result = await handler.Handle(
            new ListCallsQuery(null, null, null, null, creator.Id, RestrictToSupervisedAreas: false, RestrictToOwnOrUnassignedInAreas: false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(c => c.Priority).Should().ContainInOrder("Alta", "Media", "Baixa");
    }
}
