namespace MediaForge.Catalog.Application.DTOs;

public sealed record EditionDto(
    Guid Id,
    Guid WorkId,
    Guid CreatorId,
    string NarratorTeamName,
    string? Description,
    string? CoverUrl,
    string Language,
    bool IsDefault,
    DateTime CreatedAt);
