using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class PartChapterConfiguration : IEntityTypeConfiguration<PartChapter>
{
    public void Configure(EntityTypeBuilder<PartChapter> builder)
    {
        builder.ToTable("part_chapters");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PartId)
            .HasColumnName("part_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.StartTimeSeconds)
            .HasColumnName("start_time_seconds");

        builder.Property(x => x.EndTimeSeconds)
            .HasColumnName("end_time_seconds");

        builder.Property(x => x.Order)
            .HasColumnName("order");

        builder.HasIndex(x => x.PartId).HasDatabaseName("ix_part_chapters_part_id");
    }
}
