using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class WorkGenreConfiguration : IEntityTypeConfiguration<WorkGenre>
{
    public void Configure(EntityTypeBuilder<WorkGenre> builder)
    {
        builder.ToTable("work_genres");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.GenreId)
            .HasColumnName("genre_id")
            .IsRequired();

        builder.HasIndex(x => new { x.WorkId, x.GenreId })
            .IsUnique()
            .HasDatabaseName("ix_work_genres_work_genre");
    }
}
