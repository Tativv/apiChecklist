using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ChecklistInstanceConfiguration : IEntityTypeConfiguration<ChecklistInstance>
{
    public void Configure(EntityTypeBuilder<ChecklistInstance> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(i => new { i.AssetId, i.Date });

        builder.HasIndex(i => new { i.TemplateId, i.AssetId, i.Date }).IsUnique();

        builder.HasIndex(i => i.Status);

        builder.HasOne(i => i.Asset)
            .WithMany(a => a.ChecklistInstances)
            .HasForeignKey(i => i.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.AssignedUser)
            .WithMany(u => u.AssignedInstances)
            .HasForeignKey(i => i.AssignedUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ApprovedByUser)
            .WithMany(u => u.ApprovedInstances)
            .HasForeignKey(i => i.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.TaskExecutions)
            .WithOne(e => e.ChecklistInstance)
            .HasForeignKey(e => e.ChecklistInstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
