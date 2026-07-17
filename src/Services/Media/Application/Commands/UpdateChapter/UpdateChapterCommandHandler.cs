using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.UpdateChapter;

public sealed class UpdateChapterCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<UpdateChapterCommand, Result<ChapterDto>>
{
    public async Task<Result<ChapterDto>> Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdWithChaptersAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<ChapterDto>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure<ChapterDto>(Error.Unauthorized("You do not have access to this media asset."));

        var chapter = asset.Chapters.FirstOrDefault(c => c.Id == request.ChapterId);
        if (chapter is null)
            return Result.Failure<ChapterDto>(Error.NotFound("Chapter", request.ChapterId));

        var startTime = TimeSpan.FromSeconds(request.StartTimeSeconds);
        var endTime = request.EndTimeSeconds.HasValue ? TimeSpan.FromSeconds(request.EndTimeSeconds.Value) : (TimeSpan?)null;

        var result = chapter.Update(request.Title, startTime, endTime);
        if (result.IsFailure)
            return Result.Failure<ChapterDto>(result.Error);

        mediaAssetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(chapter.ToDto());
    }
}
