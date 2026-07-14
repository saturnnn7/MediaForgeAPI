namespace MediaForge.Media.Application.DTOs;

public sealed record UploadUrlDto(Guid AssetId, string UploadUrl, DateTime ExpiresAt);
