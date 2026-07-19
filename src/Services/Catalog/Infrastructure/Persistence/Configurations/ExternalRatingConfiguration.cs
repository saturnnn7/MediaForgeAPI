using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ExternalRatingConfiguration : IEntityTypeConfiguration<ExternalRating>
{
    public void Configure(EntityTypeBuilder<ExternalRating> builder)
    {
        builder.ToTable("external_ratings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.Source)
            .HasColumnName("source")
            .HasConversion<string>();

        builder.Property(x => x.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Score)
            .HasColumnName("score")
            .IsRequired(false);

        builder.Property(x => x.ReviewCount)
            .HasColumnName("review_count")
            .IsRequired(false);

        builder.Property(x => x.ExternalUrl)
            .HasColumnName("external_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.LastFetchedAt)
            .HasColumnName("last_fetched_at")
            .IsRequired(false);

        builder.HasIndex(x => new { x.WorkId, x.Source })
            .IsUnique()
            .HasDatabaseName("ix_external_ratings_work_source");

        builder.HasIndex(x => x.WorkId).HasDatabaseName("ix_external_ratings_work_id");
    }
}
