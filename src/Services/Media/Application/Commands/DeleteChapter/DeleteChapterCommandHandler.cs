using MediaForge.Media.Application.Abstractions;

namespace MediaForge.Media.Application.Commands.DeleteChapter;

public sealed class DeleteChapterCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<DeleteChapterCommand, Result>
{
    public async Task<Result> Handle(DeleteChapterCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdWithChaptersAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure(Error.Unauthorized("You do not have access to this media asset."));

        var result = asset.RemoveChapter(request.ChapterId);
        if (result.IsFailure)
            return result;

        mediaAssetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
