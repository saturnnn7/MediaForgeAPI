namespace MediaForge.Media.Application.Commands.DeleteChapter;

public sealed record DeleteChapterCommand(Guid AssetId, Guid ChapterId) : IRequest<Result>;
