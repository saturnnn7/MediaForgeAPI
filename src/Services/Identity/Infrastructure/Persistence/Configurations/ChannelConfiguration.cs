using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Identity.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("channels");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(c => c.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(c => c.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(c => c.SubscriberCount)
            .HasColumnName("subscriber_count")
            .HasDefaultValue(0);

        builder.HasIndex(c => c.OwnerId).IsUnique();
    }
}
