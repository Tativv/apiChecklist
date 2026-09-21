using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("schedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.FrequencyType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.WeekDay)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(s => new { s.FrequencyType, s.Active });
    }
}
