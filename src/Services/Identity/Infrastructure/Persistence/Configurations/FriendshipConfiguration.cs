using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Identity.Infrastructure.Persistence.Configurations;

public sealed class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("friendships");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(f => f.RequesterId)
            .HasColumnName("requester_id")
            .IsRequired();

        builder.Property(f => f.AddresseeId)
            .HasColumnName("addressee_id")
            .IsRequired();

        builder.Property(f => f.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(f => f.RespondedAt)
            .HasColumnName("responded_at");

        builder.HasIndex(f => new { f.RequesterId, f.AddresseeId }).IsUnique();
        builder.HasIndex(f => f.AddresseeId);
        builder.HasIndex(f => f.RequesterId);
    }
}
