using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class EditionConfiguration : IEntityTypeConfiguration<Edition>
{
    public void Configure(EntityTypeBuilder<Edition> builder)
    {
        builder.ToTable("editions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnName("creator_id")
            .IsRequired();

        builder.Property(x => x.NarratorTeamName)
            .HasColumnName("narrator_team_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.CoverUrl)
            .HasColumnName("cover_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Language)
            .HasColumnName("language")
            .HasMaxLength(10)
            .HasDefaultValue("en");

        builder.Property(x => x.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(x => x.Parts);

        builder.HasMany<Part>("_parts")
            .WithOne()
            .HasForeignKey(p => p.EditionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_parts").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.WorkId).HasDatabaseName("ix_editions_work_id");
        builder.HasIndex(x => new { x.WorkId, x.IsDefault }).HasDatabaseName("ix_editions_work_default");
    }
}
