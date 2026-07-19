using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace MediaForge.Media.Application.Commands.RequestImageUploadUrl;

public sealed class RequestImageUploadUrlCommandHandler(
    IStorageService storageService,
    IConfiguration configuration) : IRequestHandler<RequestImageUploadUrlCommand, Result<ImageUploadUrlDto>>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private static readonly string[] AllowedCategories = ["persons", "works", "series", "channels"];

    public async Task<Result<ImageUploadUrlDto>> Handle(RequestImageUploadUrlCommand request, CancellationToken cancellationToken)
    {
        if (!AllowedContentTypes.Contains(request.ContentType))
            return Result.Failure<ImageUploadUrlDto>(Error.Validation("ContentType", "Unsupported image type."));

        if (!AllowedCategories.Contains(request.Category))
            return Result.Failure<ImageUploadUrlDto>(Error.Validation("Category", "Unsupported category."));

        var objectKey = $"images/{request.Category}/{Guid.NewGuid()}/{request.FileName}";
        var expiry = TimeSpan.FromMinutes(15);
        var uploadUrl = await storageService.GenerateImageUploadUrlAsync(objectKey, cancellationToken);

        var storageServiceUrl = configuration["Storage:ServiceUrl"] ?? "http://localhost:9000";
        var imageUrl = $"{storageServiceUrl}/mediaforge-images/{objectKey}";

        return Result.Success(new ImageUploadUrlDto(uploadUrl, imageUrl, objectKey, DateTime.UtcNow.Add(expiry)));
    }
}
