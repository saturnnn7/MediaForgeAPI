namespace MediaForge.Library.Application.Commands.RemoveWorkFromList;

public sealed record RemoveWorkFromListCommand(Guid ListId, Guid WorkId) : IRequest<Result>;
