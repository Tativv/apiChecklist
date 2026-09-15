using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class ChecklistTaskEvidenceConfiguration : IEntityTypeConfiguration<ChecklistTaskEvidence>
{
    public void Configure(EntityTypeBuilder<ChecklistTaskEvidence> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(e => e.UploadedByUser)
            .WithMany(u => u.UploadedEvidences)
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
