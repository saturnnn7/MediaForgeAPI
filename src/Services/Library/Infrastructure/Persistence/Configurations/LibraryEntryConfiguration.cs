using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class LibraryEntryConfiguration : IEntityTypeConfiguration<LibraryEntry>
{
    public void Configure(EntityTypeBuilder<LibraryEntry> builder)
    {
        builder.ToTable("library_entries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(LibraryStatus.Planned);

        builder.Property(x => x.IsFavorite)
            .HasColumnName("is_favorite")
            .HasDefaultValue(false);

        builder.Property(x => x.Rating)
            .HasColumnName("rating")
            .IsRequired(false);

        builder.Property(x => x.Privacy)
            .HasColumnName("privacy")
            .HasConversion<string>()
            .HasDefaultValue(ListPrivacy.Everyone);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(x => new { x.UserId, x.WorkId })
            .IsUnique()
            .HasDatabaseName("ix_library_entries_user_work");

        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_library_entries_user_id");
        builder.HasIndex(x => x.WorkId).HasDatabaseName("ix_library_entries_work_id");
    }
}
