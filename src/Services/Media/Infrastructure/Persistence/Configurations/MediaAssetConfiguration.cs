using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Media.Infrastructure.Persistence.Configurations;

public sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_assets");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("file_size_bytes");

        builder.Property(x => x.MediaType)
            .HasColumnName("media_type")
            .HasConversion<string>();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(x => x.BucketName)
            .HasColumnName("bucket_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasColumnName("object_key")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ThumbnailUrl)
            .HasColumnName("thumbnail_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.WaveformUrl)
            .HasColumnName("waveform_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.SubtitleUrl)
            .HasColumnName("subtitle_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.TranscriptionText)
            .HasColumnName("transcription_text")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.DurationSeconds)
            .HasColumnName("duration_seconds")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.ProcessingCompletedAt)
            .HasColumnName("processing_completed_at")
            .IsRequired(false);

        builder.Property<List<string>>("_outputUrls")
            .HasColumnName("output_urls")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
                    v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                    v => v.ToList()));

        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_media_assets_user_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_media_assets_status");

        builder.Ignore(x => x.Chapters);

        builder.HasMany<Chapter>("_chapters")
            .WithOne()
            .HasForeignKey(c => c.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_chapters").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
