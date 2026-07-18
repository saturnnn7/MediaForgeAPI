using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateWork;

public sealed record UpdateWorkCommand(Guid WorkId, string Title, string? Description, string? CoverUrl, string? Language) : IRequest<Result<WorkSummaryDto>>;
