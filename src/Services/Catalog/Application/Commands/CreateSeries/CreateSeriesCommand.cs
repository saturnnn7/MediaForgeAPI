using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateSeries;

public sealed record CreateSeriesCommand(Guid ChannelId, string Title, string? Description, string? CoverUrl) : IRequest<Result<SeriesDto>>;
