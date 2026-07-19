using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class ReviewReactionConfiguration : IEntityTypeConfiguration<ReviewReaction>
{
    public void Configure(EntityTypeBuilder<ReviewReaction> builder)
    {
        builder.ToTable("review_reactions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.TargetId)
            .HasColumnName("target_id")
            .IsRequired();

        builder.Property(x => x.TargetType)
            .HasColumnName("target_type")
            .HasConversion<string>();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Emoji)
            .HasColumnName("emoji")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(x => new { x.TargetId, x.UserId, x.Emoji })
            .IsUnique()
            .HasDatabaseName("ix_review_reactions_target_user_emoji");

        builder.HasIndex(x => x.TargetId).HasDatabaseName("ix_review_reactions_target_id");
    }
}
