using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistTemplates;
using HotelChecklist.Api.Features.ChecklistTemplates.Update;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistTemplates;

// Nota: la rama "sin cambios de tareas" y la rama "tareas sin historial" de
// UpdateChecklistTemplateHandler (que mutan Tasks/TemplateSchedules in situ via RemoveRange/
// AddRange) no tienen test unitario acá para el caso general porque el proveedor InMemory de EF
// Core no soporta bien ese patrón de reemplazo de colección requerida en algunos casos (visto ya
// con Clear()+Add(); ver el historial de UpdateUserHandler/UserAreas) — están cubiertas por la
// verificación manual contra Postgres real.
public class UpdateChecklistTemplateHandlerTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly DateOnly Yesterday = Today.AddDays(-1);

    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly ScheduleInput DailySchedule = new("Daily", 1, null, null, "08:00", 0);

    private static ChecklistTemplate BuildTemplate(Guid areaId, UserRole createdByRole = UserRole.Directoria)
    {
        var templateId = Guid.NewGuid();
        return new ChecklistTemplate
        {
            Id = templateId,
            GroupId = templateId,
            IsSnapshot = false,
            Name = "Original",
            AreaId = areaId,
            EstimatedDurationMinutes = 15,
            CreatedByRole = createdByRole,
            Tasks = [new ChecklistTaskRequest("Tarea 1", null, 1, null, "Continuous", []).ToTask()]
        };
    }

    private static UpdateChecklistTemplateCommand BuildCommand(
        Guid templateId, Guid areaId, string name, string taskName, UserRole actingUserRole = UserRole.Directoria) => new(
        templateId,
        name,
        "desc",
        areaId,
        20,
        "Scheduled",
        [DailySchedule],
        [new ChecklistTaskRequest(taskName, null, 1, null, "Continuous", [])],
        actingUserRole);

    [Fact]
    public async Task Handle_NameOnlyChange_ShouldMutateInPlace_EvenWithPastHistory()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id);
        var taskId = template.Tasks.First().Id;
        var taskName = template.Tasks.First().Name;
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var pastInstance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            AssetId = Guid.NewGuid(),
            Date = Yesterday,
            Status = ChecklistStatus.Completed
        };
        db.ChecklistInstances.Add(pastInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = pastInstance.Id,
            TaskId = taskId
        });
        await db.SaveChangesAsync();

        // Mismo nombre de tarea (sin cambios en la lista de tareas) — sólo cambia el nombre del template.
        var command = BuildCommand(template.Id, area.Id, "Editado", taskName);

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        result.Value.VersionedAsNewTemplate.Should().BeFalse();
        result.Value.Id.Should().Be(template.Id);
        result.Value.Name.Should().Be("Editado");

        (await db.ChecklistTemplates.CountAsync()).Should().Be(1);
        var unchangedTask = await db.ChecklistTasks.FindAsync(taskId);
        unchangedTask.Should().NotBeNull("la tarea original no debería tocarse si la lista de tareas no cambió");
    }

    [Fact]
    public async Task Handle_TaskChange_NoHistory_ShouldMutateTasksInPlace()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id);
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var command = BuildCommand(template.Id, area.Id, "Editado", "Tarea nueva");

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        result.Value.VersionedAsNewTemplate.Should().BeFalse();
        result.Value.Id.Should().Be(template.Id);
        result.Value.Tasks.Should().ContainSingle(t => t.Name == "Tarea nueva");

        (await db.ChecklistTemplates.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_TaskChange_WithTodayPendingExecution_ShouldVersionEvenSameDay()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id);
        var originalTaskId = template.Tasks.First().Id;
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var todayInstance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            AssetId = Guid.NewGuid(),
            Date = Today,
            Status = ChecklistStatus.Pending
        };
        db.ChecklistInstances.Add(todayInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = todayInstance.Id,
            TaskId = originalTaskId
        });
        await db.SaveChangesAsync();

        var command = BuildCommand(template.Id, area.Id, "Editado", "Tarea nueva");

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        result.Value.VersionedAsNewTemplate.Should().BeTrue();
        result.Value.Id.Should().NotBe(template.Id);

        var rows = await db.ChecklistTemplates.Include(t => t.Tasks).ToListAsync();
        rows.Should().HaveCount(2);
        var frozen = rows.Single(t => t.Id == template.Id);
        frozen.IsSnapshot.Should().BeTrue();
        frozen.Tasks.Should().ContainSingle(t => t.Id == originalTaskId);

        // La instancia de hoy no se toca en absoluto: sigue apuntando a la versión congelada.
        var unchangedInstance = await db.ChecklistInstances.FindAsync(todayInstance.Id);
        unchangedInstance!.Status.Should().Be(ChecklistStatus.Pending);
        unchangedInstance.TemplateId.Should().Be(template.Id);
    }

    [Fact]
    public async Task Handle_TaskChange_WithPastExecution_ShouldFreezeOldAndCreateNewVersion()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id);
        var originalTaskId = template.Tasks.First().Id;
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var pastInstance = new ChecklistInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            AssetId = Guid.NewGuid(),
            Date = Yesterday,
            Status = ChecklistStatus.Completed
        };
        db.ChecklistInstances.Add(pastInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = pastInstance.Id,
            TaskId = originalTaskId
        });
        await db.SaveChangesAsync();

        var command = BuildCommand(template.Id, area.Id, "Editado", "Tarea nueva");

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
        result.Value.VersionedAsNewTemplate.Should().BeTrue();
        result.Value.Id.Should().NotBe(template.Id);

        var rows = await db.ChecklistTemplates.Include(t => t.Tasks).ToListAsync();
        rows.Should().HaveCount(2);

        var frozen = rows.Single(t => t.Id == template.Id);
        frozen.IsSnapshot.Should().BeTrue();
        frozen.Name.Should().Be("Original");
        frozen.Tasks.Should().ContainSingle(t => t.Id == originalTaskId);

        var live = rows.Single(t => t.Id == result.Value.Id);
        live.IsSnapshot.Should().BeFalse();
        live.GroupId.Should().Be(template.GroupId);
        live.Name.Should().Be("Editado");
    }

    [Fact]
    public async Task Handle_SnapshotTemplate_ShouldReturnConflict()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id);
        template.IsSnapshot = true;
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var command = BuildCommand(template.Id, area.Id, "Editado", "Tarea nueva");

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ChecklistTemplates.IsSnapshot");
    }

    [Fact]
    public async Task Handle_LowerHierarchyActor_ShouldBeForbidden()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id, createdByRole: UserRole.Directoria);
        var taskName = template.Tasks.First().Name;
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var command = BuildCommand(template.Id, area.Id, "Editado", taskName, actingUserRole: UserRole.Supervisor);

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ChecklistTemplates.InsufficientHierarchy");
    }

    [Fact]
    public async Task Handle_SameOrHigherHierarchyActor_ShouldSucceed()
    {
        await using var db = CreateDbContext();
        var area = new Area { Id = Guid.NewGuid(), Name = "Área" };
        var template = BuildTemplate(area.Id, createdByRole: UserRole.Supervisor);
        var taskName = template.Tasks.First().Name;
        db.Areas.Add(area);
        db.ChecklistTemplates.Add(template);
        await db.SaveChangesAsync();

        var command = BuildCommand(template.Id, area.Id, "Editado", taskName, actingUserRole: UserRole.Gerencia);

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue(result.IsFailure ? $"{result.Error.Code}: {result.Error.Message}" : "");
    }
}
