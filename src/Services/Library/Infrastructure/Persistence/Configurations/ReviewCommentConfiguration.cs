using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class ReviewCommentConfiguration : IEntityTypeConfiguration<ReviewComment>
{
    public void Configure(EntityTypeBuilder<ReviewComment> builder)
    {
        builder.ToTable("review_comments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ReviewId)
            .HasColumnName("review_id")
            .IsRequired();

        builder.Property(x => x.ParentCommentId)
            .HasColumnName("parent_comment_id")
            .IsRequired(false);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Text)
            .HasColumnName("text")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.ContainsSpoiler)
            .HasColumnName("contains_spoiler");

        builder.Property(x => x.IsEdited)
            .HasColumnName("is_edited")
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.EditedAt)
            .HasColumnName("edited_at")
            .IsRequired(false);

        builder.Property(x => x.IsHidden)
            .HasColumnName("is_hidden")
            .HasDefaultValue(false);

        builder.Property(x => x.ReportCount)
            .HasColumnName("report_count")
            .HasDefaultValue(0);

        builder.Property(x => x.Depth)
            .HasColumnName("depth");

        builder.HasIndex(x => x.ReviewId).HasDatabaseName("ix_review_comments_review_id");
        builder.HasIndex(x => x.ParentCommentId).HasDatabaseName("ix_review_comments_parent_comment_id");
    }
}
