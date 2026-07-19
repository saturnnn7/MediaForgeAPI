using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.SetExternalRating;

public sealed record SetExternalRatingCommand(
    Guid WorkId,
    ExternalRatingSource Source,
    string ExternalId,
    string? ExternalUrl) : IRequest<Result<ExternalRatingDto>>;
