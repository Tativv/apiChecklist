using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ChecklistTaskCommentConfiguration : IEntityTypeConfiguration<ChecklistTaskComment>
{
    public void Configure(EntityTypeBuilder<ChecklistTaskComment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(c => c.ChecklistTaskExecutionId);

        builder.HasOne(c => c.ChecklistTaskExecution)
            .WithMany(e => e.Comments)
            .HasForeignKey(c => c.ChecklistTaskExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.AuthorUser)
            .WithMany()
            .HasForeignKey(c => c.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
