namespace MediaForge.Catalog.Application.Commands.AddGenreToWork;

public sealed record AddGenreToWorkCommand(Guid WorkId, Guid GenreId) : IRequest<Result>;
