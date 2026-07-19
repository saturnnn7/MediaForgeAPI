namespace MediaForge.Library.Application.DTOs;

public sealed record ListeningProgressDto(
    Guid Id,
    Guid UserId,
    Guid EditionId,
    Guid PartId,
    double PositionSeconds,
    DateTime UpdatedAt);
