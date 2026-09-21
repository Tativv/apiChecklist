using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class TemplateScheduleConfiguration : IEntityTypeConfiguration<TemplateSchedule>
{
    public void Configure(EntityTypeBuilder<TemplateSchedule> builder)
    {
        builder.ToTable("template_schedules");

        builder.HasKey(ts => ts.Id);

        builder.HasIndex(ts => ts.TemplateId);

        builder.HasOne(ts => ts.Schedule)
            .WithMany()
            .HasForeignKey(ts => ts.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
