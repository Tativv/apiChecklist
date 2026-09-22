using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class UserAreaConfiguration : IEntityTypeConfiguration<UserArea>
{
    public void Configure(EntityTypeBuilder<UserArea> builder)
    {
        builder.ToTable("user_areas");

        builder.HasKey(ua => ua.Id);

        builder.HasIndex(ua => new { ua.UserId, ua.AreaId }).IsUnique();

        builder.HasOne(ua => ua.User)
            .WithMany(u => u.UserAreas)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ua => ua.Area)
            .WithMany()
            .HasForeignKey(ua => ua.AreaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
