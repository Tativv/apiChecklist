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

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin Demo",
            Email = "admin@hotelchecklist.local",
            Role = UserRole.Admin,
            PasswordHash = passwordHasher.Hash("Admin123!"),
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

        var operatorUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Operator Demo",
            Email = "operator@hotelchecklist.local",
            Role = UserRole.Operator,
            PasswordHash = passwordHasher.Hash("Operator123!"),
            Active = true
        };

        var manager = new User
        {
            Id = Guid.NewGuid(),
            Name = "Manager Demo",
            Email = "manager@hotelchecklist.local",
            Role = UserRole.Manager,
            PasswordHash = passwordHasher.Hash("Manager123!"),
            Active = true
        };

        db.Users.AddRange(admin, supervisor, operatorUser, manager);

        var areaNames = new[] { "Habitaciones", "Lobby", "Restaurante", "Piscina", "Mantenimiento" };
        var areas = areaNames.Select(name => new Area { Id = Guid.NewGuid(), Name = name }).ToList();
        db.Areas.AddRange(areas);

        var roomsArea = areas.First(a => a.Name == "Habitaciones");

        var sampleAsset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = "Habitación 101",
            Type = "Room",
            AreaId = roomsArea.Id,
            Active = true
        };
        db.Assets.Add(sampleAsset);

        var sampleTemplate = new ChecklistTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Checklist diario de habitación",
            Description = "Revisión estándar diaria de limpieza y mantenimiento de habitación.",
            AreaId = roomsArea.Id,
            RecurrenceType = ChecklistRecurrenceType.Daily,
            EstimatedDurationMinutes = 30,
            ScheduledTime = new TimeOnly(8, 0),
            RecurrenceStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Tasks =
            [
                new ChecklistTask { Id = Guid.NewGuid(), Name = "Tender cama", Description = "Cambiar sábanas y acomodar almohadas.", Order = 1 },
                new ChecklistTask { Id = Guid.NewGuid(), Name = "Limpiar baño", Order = 2 },
                new ChecklistTask { Id = Guid.NewGuid(), Name = "Reponer amenities", Order = 3 },
                new ChecklistTask { Id = Guid.NewGuid(), Name = "Revisar minibar", Order = 4 },
                new ChecklistTask { Id = Guid.NewGuid(), Name = "Verificar funcionamiento de A/C", Order = 5 }
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
