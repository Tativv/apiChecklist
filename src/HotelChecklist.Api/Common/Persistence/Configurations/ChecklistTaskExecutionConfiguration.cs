using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ChecklistTaskExecutionConfiguration : IEntityTypeConfiguration<ChecklistTaskExecution>
{
    public void Configure(EntityTypeBuilder<ChecklistTaskExecution> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(e => e.ChecklistInstanceId);

        builder.HasIndex(e => new { e.ChecklistInstanceId, e.TaskId });

        builder.HasOne(e => e.Schedule)
            .WithMany()
            .HasForeignKey(e => e.ScheduleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.CompletedByUser)
            .WithMany()
            .HasForeignKey(e => e.CompletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Evidences)
            .WithOne(ev => ev.ChecklistTaskExecution)
            .HasForeignKey(ev => ev.ChecklistTaskExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
