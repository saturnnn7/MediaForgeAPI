namespace MediaForge.Media.Application.DTOs;

public sealed record MultipartUploadInitiatedDto(Guid AssetId, string UploadId, string ObjectKey, string BucketName);
