using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateSeries;

public sealed record UpdateSeriesCommand(Guid SeriesId, string Title, string? Description, string? CoverUrl) : IRequest<Result<SeriesDto>>;
