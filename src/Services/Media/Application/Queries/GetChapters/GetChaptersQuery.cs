using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetChapters;

public sealed record GetChaptersQuery(Guid AssetId) : IRequest<Result<IReadOnlyList<ChapterDto>>>;
