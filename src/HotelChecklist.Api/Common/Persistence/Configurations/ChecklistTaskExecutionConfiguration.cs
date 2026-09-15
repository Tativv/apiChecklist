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

        builder.HasIndex(e => e.ChecklistInstanceId);

        builder.HasMany(e => e.Evidences)
            .WithOne(ev => ev.ChecklistTaskExecution)
            .HasForeignKey(ev => ev.ChecklistTaskExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
