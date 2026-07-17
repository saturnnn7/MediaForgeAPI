using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.AddChapter;

public sealed class AddChapterCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<AddChapterCommand, Result<ChapterDto>>
{
    public async Task<Result<ChapterDto>> Handle(AddChapterCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdWithChaptersAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<ChapterDto>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure<ChapterDto>(Error.Unauthorized("You do not have access to this media asset."));

        var startTime = TimeSpan.FromSeconds(request.StartTimeSeconds);
        var endTime = request.EndTimeSeconds.HasValue ? TimeSpan.FromSeconds(request.EndTimeSeconds.Value) : (TimeSpan?)null;

        var result = asset.AddChapter(request.Title, startTime, request.Order, endTime);
        if (result.IsFailure)
            return Result.Failure<ChapterDto>(result.Error);

        mediaAssetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.ToDto());
    }
}
