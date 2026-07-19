using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetProgress;

public sealed record GetProgressQuery(Guid EditionId) : IRequest<Result<IReadOnlyList<ListeningProgressDto>>>;
