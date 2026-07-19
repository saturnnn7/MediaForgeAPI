using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.RefreshExternalRating;

public sealed record RefreshExternalRatingCommand(Guid WorkId, ExternalRatingSource Source) : IRequest<Result<ExternalRatingDto>>;
