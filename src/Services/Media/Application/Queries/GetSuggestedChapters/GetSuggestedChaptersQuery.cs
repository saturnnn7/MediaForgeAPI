using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetSuggestedChapters;

public sealed record GetSuggestedChaptersQuery(Guid AssetId) : IRequest<Result<IReadOnlyList<SuggestedChapterDto>>>;
