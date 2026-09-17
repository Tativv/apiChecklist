using HotelChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelChecklist.Api.Common.Persistence.Configurations;

public sealed class TemplateAssetConfiguration : IEntityTypeConfiguration<TemplateAsset>
{
    public void Configure(EntityTypeBuilder<TemplateAsset> builder)
    {
        builder.ToTable("template_assets");

        builder.HasKey(ta => ta.Id);

        builder.HasIndex(ta => new { ta.TemplateId, ta.AssetId }).IsUnique();

        builder.HasOne(ta => ta.Asset)
            .WithMany()
            .HasForeignKey(ta => ta.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
