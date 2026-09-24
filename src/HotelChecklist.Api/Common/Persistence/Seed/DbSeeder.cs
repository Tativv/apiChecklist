using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Common.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);

        if (await db.Users.AnyAsync(cancellationToken))
            return;

        var directoria = new User
        {
            Id = Guid.NewGuid(),
            Name = "Directoria Demo",
            Email = "directoria@hotelchecklist.local",
            Role = UserRole.Directoria,
            PasswordHash = passwordHasher.Hash("Directoria123!"),
            Active = true
        };

        var supervisor = new User
        {
            Id = Guid.NewGuid(),
            Name = "Supervisor Demo",
            Email = "supervisor@hotelchecklist.local",
            Role = UserRole.Supervisor,
            PasswordHash = passwordHasher.Hash("Supervisor123!"),
            Active = true
        };

        var colaborador = new User
        {
            Id = Guid.NewGuid(),
            Name = "Colaborador Demo",
            Email = "colaborador@hotelchecklist.local",
            Role = UserRole.Colaborador,
            PasswordHash = passwordHasher.Hash("Colaborador123!"),
            Active = true
        };

        var gerencia = new User
        {
            Id = Guid.NewGuid(),
            Name = "Gerencia Demo",
            Email = "gerencia@hotelchecklist.local",
            Role = UserRole.Gerencia,
            PasswordHash = passwordHasher.Hash("Gerencia123!"),
            Active = true
        };

        db.Users.AddRange(directoria, supervisor, colaborador, gerencia);

        var areaNames = new[] { "Habitaciones", "Lobby", "Restaurante", "Piscina", "Mantenimiento" };
        var areas = areaNames.Select(name => new Area { Id = Guid.NewGuid(), Name = name }).ToList();
        db.Areas.AddRange(areas);

        var roomsArea = areas.First(a => a.Name == "Habitaciones");

        db.UserAreas.Add(new UserArea { Id = Guid.NewGuid(), UserId = supervisor.Id, AreaId = roomsArea.Id });

        var sampleAsset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = "Habitación 101",
            Type = "Room",
            AreaId = roomsArea.Id,
            Active = true
        };
        db.Assets.Add(sampleAsset);

        var now = DateTimeOffset.UtcNow;

        Schedule DailyAt(int hour, int minute) => new()
        {
            Id = Guid.NewGuid(),
            FrequencyType = ScheduleFrequencyType.Daily,
            IntervalValue = 1,
            TimeOfDay = new TimeOnly(hour, minute),
            Active = true,
            CreatedAtUtc = now
        };

        ChecklistTask ScheduledTask(string name, int order, int hour, int minute, string? description = null) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Order = order,
            ExecutionMode = TaskExecutionMode.Scheduled,
            TaskSchedules = [new TaskSchedule { Id = Guid.NewGuid(), Schedule = DailyAt(hour, minute) }]
        };

        var sampleTemplate = new ChecklistTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Checklist diario de habitación",
            Description = "Revisión estándar diaria de limpieza y mantenimiento de habitación.",
            AreaId = roomsArea.Id,
            TemplateSchedules = [new TemplateSchedule { Id = Guid.NewGuid(), Schedule = DailyAt(8, 0) }],
            Tasks =
            [
                ScheduledTask("Tender cama", 1, 8, 30, "Cambiar sábanas y acomodar almohadas."),
                ScheduledTask("Limpiar baño", 2, 8, 30),
                ScheduledTask("Reponer amenities", 3, 8, 30),
                ScheduledTask("Revisar minibar", 4, 8, 30),
                ScheduledTask("Verificar funcionamiento de A/C", 5, 8, 30),
                new ChecklistTask
                {
                    Id = Guid.NewGuid(),
                    Name = "Atención cordial",
                    Order = 6,
                    ExecutionMode = TaskExecutionMode.Continuous
                }
            ]
        };
        db.ChecklistTemplates.Add(sampleTemplate);
        db.TemplateAssets.Add(new TemplateAsset
        {
            Id = Guid.NewGuid(),
            TemplateId = sampleTemplate.Id,
            AssetId = sampleAsset.Id,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
