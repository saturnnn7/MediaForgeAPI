namespace MediaForge.Catalog.Application.Commands.SetDefaultEdition;

public sealed record SetDefaultEditionCommand(Guid EditionId) : IRequest<Result>;
