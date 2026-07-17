using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.AddChapter;

public sealed record AddChapterCommand(
    Guid AssetId,
    string Title,
    double StartTimeSeconds,
    int Order,
    double? EndTimeSeconds) : IRequest<Result<ChapterDto>>;
