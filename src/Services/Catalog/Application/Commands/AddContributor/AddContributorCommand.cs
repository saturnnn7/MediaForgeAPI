namespace MediaForge.Catalog.Application.Commands.AddContributor;

public sealed record AddContributorCommand(Guid WorkId, Guid PersonId, ContributorRole Role, int DisplayOrder = 0) : IRequest<Result>;
