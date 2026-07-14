using MassTransit;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Shared.Contracts.Events.Media;

namespace MediaForge.Media.Application.Commands.ConfirmUpload;

public sealed class ConfirmUploadCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IMediaUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<ConfirmUploadCommand, Result>
{
    public async Task<Result> Handle(ConfirmUploadCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
        {
            return Result.Failure(Error.NotFound("MediaAsset", request.AssetId));
        }

        if (asset.UserId != userContext.UserId)
        {
            return Result.Failure(Error.Unauthorized("You do not have access to this media asset."));
        }

        var result = asset.ConfirmUpload();
        if (result.IsFailure)
        {
            return result;
        }

        mediaAssetRepository.Update(asset);

        await publishEndpoint.Publish(
            new MediaUploadedEvent(
                Id: Guid.NewGuid(),
                OccurredOn: DateTime.UtcNow,
                CorrelationId: Guid.NewGuid(),
                AssetId: asset.Id,
                UserId: asset.UserId,
                BucketName: asset.BucketName,
                ObjectKey: asset.ObjectKey,
                ContentType: asset.ContentType,
                FileSizeBytes: asset.FileSizeBytes),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
