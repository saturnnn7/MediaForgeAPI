using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class PartAssetConfiguration : IEntityTypeConfiguration<PartAsset>
{
    public void Configure(EntityTypeBuilder<PartAsset> builder)
    {
        builder.ToTable("part_assets");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PartId)
            .HasColumnName("part_id")
            .IsRequired();

        builder.Property(x => x.MediaAssetId)
            .HasColumnName("media_asset_id")
            .IsRequired();

        builder.Property(x => x.SequenceOrder)
            .HasColumnName("sequence_order");

        builder.HasIndex(x => new { x.PartId, x.MediaAssetId }).IsUnique().HasDatabaseName("ix_part_assets_part_media_asset");
        builder.HasIndex(x => x.PartId).HasDatabaseName("ix_part_assets_part_id");
    }
}
