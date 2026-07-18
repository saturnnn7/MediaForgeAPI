namespace MediaForge.Catalog.Application.Commands.RemoveContributor;

public sealed record RemoveContributorCommand(Guid WorkId, Guid PersonId, ContributorRole Role) : IRequest<Result>;
