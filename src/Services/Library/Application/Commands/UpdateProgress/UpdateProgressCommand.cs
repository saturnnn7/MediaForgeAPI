using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.UpdateProgress;

public sealed record UpdateProgressCommand(Guid EditionId, Guid PartId, double PositionSeconds) : IRequest<Result<ListeningProgressDto>>;
