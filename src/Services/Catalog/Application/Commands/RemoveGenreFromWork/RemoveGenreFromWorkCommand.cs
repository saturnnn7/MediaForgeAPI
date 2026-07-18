namespace MediaForge.Catalog.Application.Commands.RemoveGenreFromWork;

public sealed record RemoveGenreFromWorkCommand(Guid WorkId, Guid GenreId) : IRequest<Result>;
