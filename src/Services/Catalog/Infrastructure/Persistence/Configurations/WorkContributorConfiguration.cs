using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Catalog.Infrastructure.Persistence.Configurations;

public sealed class WorkContributorConfiguration : IEntityTypeConfiguration<WorkContributor>
{
    public void Configure(EntityTypeBuilder<WorkContributor> builder)
    {
        builder.ToTable("work_contributors");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.PersonId)
            .HasColumnName("person_id")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<string>();

        builder.Property(x => x.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        builder.HasIndex(x => new { x.WorkId, x.PersonId, x.Role })
            .IsUnique()
            .HasDatabaseName("ix_work_contributors_work_person_role");

        builder.HasIndex(x => x.PersonId).HasDatabaseName("ix_work_contributors_person_id");
    }
}
