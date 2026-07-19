using MassTransit;
using MediaForge.Shared.Infrastructure.Persistence;
using MediatR;

namespace MediaForge.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options, IPublisher publisher)
    : BaseDbContext(options, publisher), ICatalogUnitOfWork
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Series> Series => Set<Series>();
    public DbSet<Work> Works => Set<Work>();
    public DbSet<WorkContributor> WorkContributors => Set<WorkContributor>();
    public DbSet<WorkGenre> WorkGenres => Set<WorkGenre>();
    public DbSet<Edition> Editions => Set<Edition>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<PartChapter> PartChapters => Set<PartChapter>();
    public DbSet<PartAsset> PartAssets => Set<PartAsset>();
    public DbSet<WorkRequest> WorkRequests => Set<WorkRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
