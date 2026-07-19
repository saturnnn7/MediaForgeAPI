using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Identity.Infrastructure.Persistence.Configurations;

public sealed class AuthorFollowConfiguration : IEntityTypeConfiguration<AuthorFollow>
{
    public void Configure(EntityTypeBuilder<AuthorFollow> builder)
    {
        builder.ToTable("author_follows");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(f => f.PersonId)
            .HasColumnName("person_id")
            .IsRequired();

        builder.Property(f => f.FollowedAt)
            .HasColumnName("followed_at");

        builder.HasIndex(f => new { f.UserId, f.PersonId }).IsUnique();
        builder.HasIndex(f => f.UserId);
        builder.HasIndex(f => f.PersonId);
    }
}
