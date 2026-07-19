using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateEdition;

public sealed record UpdateEditionCommand(
    Guid EditionId,
    string NarratorTeamName,
    string? Description,
    string? CoverUrl,
    string? Language) : IRequest<Result<EditionDto>>;
