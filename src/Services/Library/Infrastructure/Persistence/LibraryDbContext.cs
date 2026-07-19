using MassTransit;
using MediaForge.Shared.Infrastructure.Persistence;
using MediatR;

namespace MediaForge.Library.Infrastructure.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options, IPublisher publisher)
    : BaseDbContext(options, publisher), ILibraryUnitOfWork
{
    public DbSet<LibraryEntry> LibraryEntries => Set<LibraryEntry>();
    public DbSet<ListeningProgress> ListeningProgresses => Set<ListeningProgress>();
    public DbSet<UserList> UserLists => Set<UserList>();
    public DbSet<UserListItem> UserListItems => Set<UserListItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewComment> ReviewComments => Set<ReviewComment>();
    public DbSet<ReviewReaction> ReviewReactions => Set<ReviewReaction>();
    public DbSet<CommentReport> CommentReports => Set<CommentReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
