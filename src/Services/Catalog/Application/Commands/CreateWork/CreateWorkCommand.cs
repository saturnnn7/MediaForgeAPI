using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateWork;

public sealed record CreateWorkCommand(
    Guid ChannelId,
    WorkType WorkType,
    string Title,
    string? Description,
    string? CoverUrl,
    string? Language,
    Guid? SeriesId) : IRequest<Result<WorkSummaryDto>>;
