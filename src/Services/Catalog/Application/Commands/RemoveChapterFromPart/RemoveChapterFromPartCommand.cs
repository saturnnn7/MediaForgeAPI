namespace MediaForge.Catalog.Application.Commands.RemoveChapterFromPart;

public sealed record RemoveChapterFromPartCommand(Guid PartId, Guid ChapterId) : IRequest<Result>;
