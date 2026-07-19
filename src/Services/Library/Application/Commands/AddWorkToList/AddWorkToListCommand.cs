namespace MediaForge.Library.Application.Commands.AddWorkToList;

public sealed record AddWorkToListCommand(Guid ListId, Guid WorkId, int DisplayOrder = 0) : IRequest<Result>;
