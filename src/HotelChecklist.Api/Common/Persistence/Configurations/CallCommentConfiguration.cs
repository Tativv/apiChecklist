using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class CallCommentConfiguration : IEntityTypeConfiguration<CallComment>
{
    public void Configure(EntityTypeBuilder<CallComment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Text)
            .HasMaxLength(2000);

        builder.Property(c => c.FilePath)
            .HasMaxLength(500);

        builder.Property(c => c.FileName)
            .HasMaxLength(300);

        builder.Property(c => c.ContentType)
            .HasMaxLength(100);

        builder.HasIndex(c => c.CallId);

        builder.HasOne(c => c.Call)
            .WithMany(call => call.Comments)
            .HasForeignKey(c => c.CallId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.AuthorUser)
            .WithMany()
            .HasForeignKey(c => c.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
