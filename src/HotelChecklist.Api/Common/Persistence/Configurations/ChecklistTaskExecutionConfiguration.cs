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

        builder.HasIndex(e => e.AssignedUserId);

        builder.HasOne(e => e.Schedule)
            .WithMany()
            .HasForeignKey(e => e.ScheduleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.AssignedUser)
            .WithMany()
            .HasForeignKey(e => e.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ExecutedByUser)
            .WithMany()
            .HasForeignKey(e => e.ExecutedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ApprovedByUser)
            .WithMany()
            .HasForeignKey(e => e.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Evidences)
            .WithOne(ev => ev.ChecklistTaskExecution)
            .HasForeignKey(ev => ev.ChecklistTaskExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
