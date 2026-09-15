using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(a => a.Name).IsUnique();

        builder.HasMany(a => a.Assets)
            .WithOne(a => a.Area)
            .HasForeignKey(a => a.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.ChecklistTemplates)
            .WithOne(t => t.Area)
            .HasForeignKey(t => t.AreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
