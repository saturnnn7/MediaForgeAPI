using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class ListeningProgressConfiguration : IEntityTypeConfiguration<ListeningProgress>
{
    public void Configure(EntityTypeBuilder<ListeningProgress> builder)
    {
        builder.ToTable("listening_progresses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.EditionId)
            .HasColumnName("edition_id")
            .IsRequired();

        builder.Property(x => x.PartId)
            .HasColumnName("part_id")
            .IsRequired();

        builder.Property(x => x.PositionSeconds)
            .HasColumnName("position_seconds");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(x => new { x.UserId, x.EditionId, x.PartId })
            .IsUnique()
            .HasDatabaseName("ix_listening_progress_user_edition_part");

        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_listening_progress_user_id");
    }
}
