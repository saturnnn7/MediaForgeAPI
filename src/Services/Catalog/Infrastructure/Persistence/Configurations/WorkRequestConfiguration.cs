using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class WorkRequestConfiguration : IEntityTypeConfiguration<WorkRequest>
{
    public void Configure(EntityTypeBuilder<WorkRequest> builder)
    {
        builder.ToTable("work_requests");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.RequesterId)
            .HasColumnName("requester_id")
            .IsRequired();

        builder.Property(x => x.WorkType)
            .HasColumnName("work_type")
            .HasConversion<string>();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.AuthorNames)
            .HasColumnName("author_names")
            .HasMaxLength(500)
            .IsRequired(false);

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

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(WorkRequestStatus.Pending);

        builder.Property(x => x.AdminNote)
            .HasColumnName("admin_note")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.ResultingWorkId)
            .HasColumnName("resulting_work_id")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.ReviewedAt)
            .HasColumnName("reviewed_at")
            .IsRequired(false);

        builder.HasIndex(x => x.RequesterId).HasDatabaseName("ix_work_requests_requester_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_work_requests_status");
    }
}
