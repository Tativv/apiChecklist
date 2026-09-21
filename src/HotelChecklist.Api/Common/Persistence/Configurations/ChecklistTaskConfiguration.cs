using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ChecklistTaskConfiguration : IEntityTypeConfiguration<ChecklistTask>
{
    public void Configure(EntityTypeBuilder<ChecklistTask> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.ExecutionMode)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(t => new { t.TemplateId, t.Order });

        builder.HasMany(t => t.Executions)
            .WithOne(e => e.Task)
            .HasForeignKey(e => e.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.TaskSchedules)
            .WithOne(ts => ts.Task)
            .HasForeignKey(ts => ts.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
