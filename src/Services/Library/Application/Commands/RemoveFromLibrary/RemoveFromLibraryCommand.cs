namespace MediaForge.Library.Application.Commands.RemoveFromLibrary;

public sealed record RemoveFromLibraryCommand(Guid WorkId) : IRequest<Result>;
