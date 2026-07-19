using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Identity.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(u => u.AvatarUrl)
            .HasColumnName("avatar_url")
            .IsRequired(false);

        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("is_email_verified")
            .HasDefaultValue(false);

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasDefaultValue(UserRole.Listener);
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(u => u.IsBanned)
            .HasColumnName("is_banned")
            .HasDefaultValue(false);

        builder.Property(u => u.BanReason)
            .HasColumnName("ban_reason")
            .IsRequired(false);

        builder.OwnsMany(u => u.RefreshTokens, rt =>
        {
            rt.ToTable("refresh_tokens");

            rt.WithOwner().HasForeignKey("application_user_id");

            rt.Property(t => t.Token)
                .HasColumnName("token")
                .HasMaxLength(500)
                .IsRequired();

            rt.Property(t => t.ExpiresAt)
                .HasColumnName("expires_at");

            rt.Property(t => t.CreatedAt)
                .HasColumnName("created_at");

            rt.Property(t => t.IsRevoked)
                .HasColumnName("is_revoked");

            rt.HasKey("application_user_id", "Token");
        });

        builder.Navigation(u => u.RefreshTokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(u => u.Channel)
            .WithOne()
            .HasForeignKey<Channel>(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
