using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.HasKey(so => so.Id);

        builder.Property(so => so.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(so => so.Description)
            .HasMaxLength(2000);

        builder.Property(so => so.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(so => so.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(so => so.AreaId);
        builder.HasIndex(so => so.AssetId);
        builder.HasIndex(so => so.Status);
        builder.HasIndex(so => so.AssignedUserId);
        builder.HasIndex(so => so.CallId).IsUnique();

        builder.HasOne(so => so.Area)
            .WithMany()
            .HasForeignKey(so => so.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Asset)
            .WithMany()
            .HasForeignKey(so => so.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.CreatedByUser)
            .WithMany()
            .HasForeignKey(so => so.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.AssignedUser)
            .WithMany()
            .HasForeignKey(so => so.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Call)
            .WithMany()
            .HasForeignKey(so => so.CallId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
