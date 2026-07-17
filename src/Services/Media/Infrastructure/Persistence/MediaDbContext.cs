using MassTransit;
using MediaForge.Shared.Infrastructure.Persistence;
using MediatR;

namespace MediaForge.Media.Infrastructure.Persistence;

public sealed class MediaDbContext(DbContextOptions<MediaDbContext> options, IPublisher publisher)
    : BaseDbContext(options, publisher), IMediaUnitOfWork
{
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<Chapter> Chapters => Set<Chapter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediaDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
