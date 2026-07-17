using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.InitiateMultipartUpload;

public sealed class InitiateMultipartUploadCommandHandler(
    IUserContextService userContext,
    IIdentityGrpcClient identityGrpcClient,
    IStorageService storageService,
    IMediaAssetRepository mediaAssetRepository,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<InitiateMultipartUploadCommand, Result<MultipartUploadInitiatedDto>>
{
    private const long MaxFileSizeBytes = 10_737_418_240L;
    private const string BucketName = "mediaforge-media";

    private static readonly string[] AllowedContentTypes =
    [
        "video/mp4",
        "video/quicktime",
        "audio/mpeg",
        "audio/wav",
        "audio/ogg"
    ];

    public async Task<Result<MultipartUploadInitiatedDto>> Handle(InitiateMultipartUploadCommand request, CancellationToken cancellationToken)
    {
        if (!AllowedContentTypes.Contains(request.ContentType))
            return Result.Failure<MultipartUploadInitiatedDto>(Error.Validation("ContentType", "Unsupported media type."));

        if (request.FileSizeBytes <= 0 || request.FileSizeBytes > MaxFileSizeBytes)
            return Result.Failure<MultipartUploadInitiatedDto>(Error.Validation("FileSizeBytes", "File size is invalid."));

        var user = await identityGrpcClient.GetUserByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<MultipartUploadInitiatedDto>(Error.Unauthorized("User not found in identity service."));

        if (userContext.Role != "creator" && userContext.Role != "admin")
            return Result.Failure<MultipartUploadInitiatedDto>(Error.Unauthorized("Only Creators can upload media."));

        var mediaType = request.ContentType.StartsWith("video/", StringComparison.Ordinal) ? MediaType.Video : MediaType.Audio;
        var objectKey = $"{userContext.UserId}/{Guid.NewGuid()}/{request.FileName}";

        var assetResult = MediaAsset.Create(
            userContext.UserId,
            request.FileName,
            request.ContentType,
            request.FileSizeBytes,
            mediaType,
            BucketName,
            objectKey);

        if (assetResult.IsFailure)
            return Result.Failure<MultipartUploadInitiatedDto>(assetResult.Error);

        var asset = assetResult.Value;
        var uploadId = await storageService.InitiateMultipartUploadAsync(BucketName, objectKey, request.ContentType, cancellationToken);

        await mediaAssetRepository.AddAsync(asset, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new MultipartUploadInitiatedDto(asset.Id, uploadId, objectKey, BucketName));
    }
}
