using MassTransit;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;
using MediaForge.Shared.Contracts.Events.Media;

namespace MediaForge.Media.Application.Commands.CompleteMultipartUpload;

public sealed class CompleteMultipartUploadCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IStorageService storageService,
    IMediaUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CompleteMultipartUploadCommand, Result<MultipartUploadCompletedDto>>
{
    public async Task<Result<MultipartUploadCompletedDto>> Handle(CompleteMultipartUploadCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<MultipartUploadCompletedDto>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure<MultipartUploadCompletedDto>(Error.Unauthorized("You do not have access to this media asset."));

        await storageService.CompleteMultipartUploadAsync(
            asset.BucketName, asset.ObjectKey, request.UploadId, request.Parts, cancellationToken);

        var result = asset.ConfirmUpload();
        if (result.IsFailure)
            return Result.Failure<MultipartUploadCompletedDto>(result.Error);

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

        return Result.Success(new MultipartUploadCompletedDto(asset.Id, "Processing"));
    }
}
