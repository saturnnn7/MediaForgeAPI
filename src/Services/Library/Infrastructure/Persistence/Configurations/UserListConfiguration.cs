using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class UserListConfiguration : IEntityTypeConfiguration<UserList>
{
    public void Configure(EntityTypeBuilder<UserList> builder)
    {
        builder.ToTable("user_lists");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Privacy)
            .HasColumnName("privacy")
            .HasConversion<string>()
            .HasDefaultValue(ListPrivacy.Everyone);

        builder.Property(x => x.IsSystem)
            .HasColumnName("is_system")
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(x => x.Items);

        builder.HasMany<UserListItem>("_items")
            .WithOne()
            .HasForeignKey(x => x.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_items").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.UserId, x.Slug }).IsUnique().HasDatabaseName("ix_user_lists_user_slug");
        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_user_lists_user_id");
    }
}
