using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaForge.Library.Infrastructure.Persistence.Configurations;

public sealed class UserListItemConfiguration : IEntityTypeConfiguration<UserListItem>
{
    public void Configure(EntityTypeBuilder<UserListItem> builder)
    {
        builder.ToTable("user_list_items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ListId)
            .HasColumnName("list_id")
            .IsRequired();

        builder.Property(x => x.WorkId)
            .HasColumnName("work_id")
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        builder.Property(x => x.AddedAt)
            .HasColumnName("added_at");

        builder.HasIndex(x => new { x.ListId, x.WorkId }).IsUnique().HasDatabaseName("ix_user_list_items_list_work");
        builder.HasIndex(x => x.ListId).HasDatabaseName("ix_user_list_items_list_id");
    }
}
