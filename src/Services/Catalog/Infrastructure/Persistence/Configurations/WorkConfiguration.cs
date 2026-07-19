using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class WorkConfiguration : IEntityTypeConfiguration<Work>
{
    public void Configure(EntityTypeBuilder<Work> builder)
    {
        builder.ToTable("works");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ChannelId)
            .HasColumnName("channel_id")
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnName("creator_id")
            .IsRequired();

        builder.Property(x => x.SeriesId)
            .HasColumnName("series_id")
            .IsRequired(false);

        builder.Property(x => x.WorkType)
            .HasColumnName("work_type")
            .HasConversion<string>();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CoverUrl)
            .HasColumnName("cover_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Language)
            .HasColumnName("language")
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(x => x.PublishedAt)
            .HasColumnName("published_at")
            .IsRequired(false);

        builder.Property(x => x.IsPublished)
            .HasColumnName("is_published")
            .HasDefaultValue(false);

        builder.Property(x => x.IsPrivate)
            .HasColumnName("is_private")
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(x => x.Contributors);
        builder.Ignore(x => x.Genres);

        builder.HasMany<WorkContributor>("_contributors")
            .WithOne()
            .HasForeignKey(x => x.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<WorkGenre>("_genres")
            .WithOne()
            .HasForeignKey(x => x.WorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_contributors").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation("_genres").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.ChannelId).HasDatabaseName("ix_works_channel_id");
        builder.HasIndex(x => x.SeriesId).HasDatabaseName("ix_works_series_id");
        builder.HasIndex(x => x.WorkType).HasDatabaseName("ix_works_work_type");
        builder.HasIndex(x => x.IsPublished).HasDatabaseName("ix_works_is_published");
    }
}
