using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdatePart;

public sealed record UpdatePartCommand(Guid PartId, string Title, string? Description, string? CoverUrl) : IRequest<Result<PartSummaryDto>>;
