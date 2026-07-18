using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Identity.Infrastructure.Persistence.Configurations;

public sealed class ChannelSubscriptionConfiguration : IEntityTypeConfiguration<ChannelSubscription>
{
    public void Configure(EntityTypeBuilder<ChannelSubscription> builder)
    {
        builder.ToTable("channel_subscriptions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(s => s.ChannelId)
            .HasColumnName("channel_id")
            .IsRequired();

        builder.Property(s => s.SubscriberId)
            .HasColumnName("subscriber_id")
            .IsRequired();

        builder.Property(s => s.SubscribedAt)
            .HasColumnName("subscribed_at");

        builder.HasOne<Channel>()
            .WithMany()
            .HasForeignKey(s => s.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.ChannelId, s.SubscriberId }).IsUnique();
        builder.HasIndex(s => s.SubscriberId);
    }
}
