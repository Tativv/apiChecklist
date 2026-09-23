using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ChecklistTemplateConfiguration : IEntityTypeConfiguration<ChecklistTemplate>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.HasIndex(t => t.AreaId);

        builder.HasIndex(t => new { t.GroupId, t.IsSnapshot });

        builder.HasIndex(t => t.GroupId)
            .IsUnique()
            .HasFilter("is_snapshot = false");

        builder.HasMany(t => t.Tasks)
            .WithOne(task => task.Template)
            .HasForeignKey(task => task.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Instances)
            .WithOne(i => i.Template)
            .HasForeignKey(i => i.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.TemplateAssets)
            .WithOne(ta => ta.Template)
            .HasForeignKey(ta => ta.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TemplateSchedules)
            .WithOne(ts => ts.Template)
            .HasForeignKey(ts => ts.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
