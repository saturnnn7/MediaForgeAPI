using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreatePart;

public sealed record CreatePartCommand(
    Guid EditionId,
    string Title,
    string? Description,
    int OrderMajor,
    int OrderMinor = 0,
    PartType PartType = PartType.Regular,
    string? CoverUrl = null) : IRequest<Result<PartSummaryDto>>;
