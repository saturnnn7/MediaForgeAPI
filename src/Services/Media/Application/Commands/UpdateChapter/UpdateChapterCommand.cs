using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.UpdateChapter;

public sealed record UpdateChapterCommand(
    Guid AssetId,
    Guid ChapterId,
    string Title,
    double StartTimeSeconds,
    double? EndTimeSeconds) : IRequest<Result<ChapterDto>>;
