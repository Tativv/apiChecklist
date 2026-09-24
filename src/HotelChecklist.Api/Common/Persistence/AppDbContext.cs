using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Common.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Area> Areas => Set<Area>();

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();

    public DbSet<ChecklistTask> ChecklistTasks => Set<ChecklistTask>();

    public DbSet<TemplateAsset> TemplateAssets => Set<TemplateAsset>();

    public DbSet<Schedule> Schedules => Set<Schedule>();

    public DbSet<TemplateSchedule> TemplateSchedules => Set<TemplateSchedule>();

    public DbSet<TaskSchedule> TaskSchedules => Set<TaskSchedule>();

    public DbSet<ChecklistInstance> ChecklistInstances => Set<ChecklistInstance>();

    public DbSet<ChecklistTaskExecution> ChecklistTaskExecutions => Set<ChecklistTaskExecution>();

    public DbSet<ChecklistTaskComment> ChecklistTaskComments => Set<ChecklistTaskComment>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserArea> UserAreas => Set<UserArea>();

    public DbSet<Call> Calls => Set<Call>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAtUtc = now;

            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UpdatedAtUtc = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
