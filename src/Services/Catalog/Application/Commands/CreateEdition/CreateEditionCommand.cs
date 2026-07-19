using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateEdition;

public sealed record CreateEditionCommand(
    Guid WorkId,
    string NarratorTeamName,
    string? Description,
    string? CoverUrl,
    string Language = "en") : IRequest<Result<EditionDto>>;
