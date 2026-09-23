using FluentAssertions;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Api.Features.ChecklistTemplates;
using HotelChecklist.Api.Features.ChecklistTemplates.Update;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.UnitTests.Features.ChecklistTemplates;

// Nota: la rama "sin historial" de UpdateChecklistTemplateHandler (mutar en el lugar via
// Tasks.Clear()+Add()) no tiene test unitario acá porque el proveedor InMemory de EF Core no
// soporta ese patrón de reemplazo de colección requerida (falla con DbUpdateConcurrencyException
// incluso en un repro mínimo sin lógica del handler de por medio) — es preexistente, no cambia en
// esta feature, y ya está cubierta por la verificación manual contra Postgres real.
public class UpdateChecklistTemplateHandlerTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly DateOnly Yesterday = Today.AddDays(-1);

    private static AppDbContext CreateDbContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly ScheduleInput DailySchedule = new("Daily", 1, null, null, "08:00", 0);

    private static ChecklistTemplate BuildTemplate(Guid areaId)
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
            Tasks = [new ChecklistTaskRequest("Tarea 1", null, 1, "Continuous", []).ToTask()]
        };
    }

    private static UpdateChecklistTemplateCommand BuildUpdateCommand(Guid templateId, Guid areaId) => new(
        templateId,
        "Editado",
        "desc",
        areaId,
        20,
        [DailySchedule],
        [new ChecklistTaskRequest("Tarea editada", null, 1, "Continuous", [])]);

    [Fact]
    public async Task Handle_TemplateWithPastExecutions_ShouldFreezeOldAndCreateNewVersion()
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
            Status = ChecklistStatus.Reviewed
        };
        db.ChecklistInstances.Add(pastInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = pastInstance.Id,
            TaskId = originalTaskId
        });
        await db.SaveChangesAsync();

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(BuildUpdateCommand(template.Id, area.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.VersionedAsNewTemplate.Should().BeTrue();
        result.Value.Id.Should().NotBe(template.Id);
        result.Value.Name.Should().Be("Editado");

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
    public async Task Handle_OnlyTodayPendingInstance_ShouldMutateInPlaceAndRegenerateExecutions()
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
            Status = ChecklistStatus.Approved
        };
        db.ChecklistInstances.Add(todayInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = todayInstance.Id,
            TaskId = originalTaskId,
            AssignedUserId = Guid.NewGuid()
        });
        await db.SaveChangesAsync();

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(BuildUpdateCommand(template.Id, area.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.VersionedAsNewTemplate.Should().BeFalse();
        result.Value.Id.Should().Be(template.Id);

        (await db.ChecklistTemplates.CountAsync()).Should().Be(1);

        var refreshedInstance = await db.ChecklistInstances.FindAsync(todayInstance.Id);
        refreshedInstance!.Status.Should().Be(ChecklistStatus.Pending);

        var executions = await db.ChecklistTaskExecutions.Where(e => e.ChecklistInstanceId == todayInstance.Id).ToListAsync();
        executions.Should().ContainSingle();
        executions[0].TaskId.Should().NotBe(originalTaskId);
        executions[0].AssignedUserId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_TodayInProgressInstance_ShouldMutateAndResetToPending()
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
            Status = ChecklistStatus.InProgress,
            StartedAt = DateTimeOffset.UtcNow
        };
        db.ChecklistInstances.Add(todayInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = todayInstance.Id,
            TaskId = originalTaskId,
            Status = TaskExecutionStatus.Completed
        });
        await db.SaveChangesAsync();

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(BuildUpdateCommand(template.Id, area.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var refreshedInstance = await db.ChecklistInstances.FindAsync(todayInstance.Id);
        refreshedInstance!.Status.Should().Be(ChecklistStatus.Pending);
        refreshedInstance.StartedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(ChecklistStatus.Completed)]
    [InlineData(ChecklistStatus.Reviewed)]
    public async Task Handle_TodayFinishedInstance_ShouldReturnConflictAskingToReopen(ChecklistStatus status)
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
            Status = status
        };
        db.ChecklistInstances.Add(todayInstance);
        db.ChecklistTaskExecutions.Add(new ChecklistTaskExecution
        {
            Id = Guid.NewGuid(),
            ChecklistInstanceId = todayInstance.Id,
            TaskId = originalTaskId,
            Status = TaskExecutionStatus.Completed
        });
        await db.SaveChangesAsync();

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(BuildUpdateCommand(template.Id, area.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ChecklistTemplates.TodayInstanceNotReopened");

        // No debe haber mutado nada: ni el template, ni la instancia de hoy.
        var unchangedTemplate = await db.ChecklistTemplates.FindAsync(template.Id);
        unchangedTemplate!.Name.Should().Be("Original");
        var unchangedInstance = await db.ChecklistInstances.FindAsync(todayInstance.Id);
        unchangedInstance!.Status.Should().Be(status);
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

        var handler = new UpdateChecklistTemplateHandler(db);
        var result = await handler.Handle(BuildUpdateCommand(template.Id, area.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ChecklistTemplates.IsSnapshot");
    }
}
