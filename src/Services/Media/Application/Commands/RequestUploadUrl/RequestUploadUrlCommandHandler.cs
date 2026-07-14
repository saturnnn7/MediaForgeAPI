using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.RequestUploadUrl;

public sealed class RequestUploadUrlCommandHandler(
    IUserContextService userContext,
    IIdentityGrpcClient identityGrpcClient,
    IStorageService storageService,
    IMediaAssetRepository mediaAssetRepository,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<RequestUploadUrlCommand, Result<UploadUrlDto>>
{
    private const long MaxFileSizeBytes = 2_147_483_648L;
    private const string BucketName = "mediaforge-media";

    private static readonly string[] AllowedContentTypes =
    [
        "video/mp4",
        "video/quicktime",
        "audio/mpeg",
        "audio/wav",
        "audio/ogg"
    ];

    public async Task<Result<UploadUrlDto>> Handle(RequestUploadUrlCommand request, CancellationToken cancellationToken)
    {
        if (!AllowedContentTypes.Contains(request.ContentType))
            return Result.Failure<UploadUrlDto>(Error.Validation("ContentType", "Unsupported media type."));

        if (request.FileSizeBytes <= 0 || request.FileSizeBytes > MaxFileSizeBytes)
            return Result.Failure<UploadUrlDto>(Error.Validation("FileSizeBytes", "File size is invalid."));

        var user = await identityGrpcClient.GetUserByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<UploadUrlDto>(Error.Unauthorized("User not found in identity service."));

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
            return Result.Failure<UploadUrlDto>(assetResult.Error);

        var asset = assetResult.Value;
        var expiry = TimeSpan.FromMinutes(15);
        var uploadUrl = await storageService.GenerateUploadUrlAsync(BucketName, objectKey, request.ContentType, expiry, cancellationToken);

        await mediaAssetRepository.AddAsync(asset, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UploadUrlDto(asset.Id, uploadUrl, DateTime.UtcNow.Add(expiry)));
    }
}
