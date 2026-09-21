using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class TaskScheduleConfiguration : IEntityTypeConfiguration<TaskSchedule>
{
    public void Configure(EntityTypeBuilder<TaskSchedule> builder)
    {
        builder.ToTable("task_schedules");

        builder.HasKey(ts => ts.Id);

        builder.HasIndex(ts => ts.TaskId);

        builder.HasOne(ts => ts.Schedule)
            .WithMany()
            .HasForeignKey(ts => ts.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
