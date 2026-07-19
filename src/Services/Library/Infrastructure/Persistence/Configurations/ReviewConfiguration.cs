using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
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

        builder.Ignore(x => x.Comments);

        builder.HasMany<ReviewComment>("_comments")
            .WithOne()
            .HasForeignKey(x => x.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_comments").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.UserId, x.WorkId })
            .IsUnique()
            .HasDatabaseName("ix_reviews_user_work");

        builder.HasIndex(x => x.WorkId).HasDatabaseName("ix_reviews_work_id");
    }
}
