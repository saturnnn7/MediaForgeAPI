namespace MediaForge.Media.Application.DTOs;

public sealed record ImageUploadUrlDto(string UploadUrl, string ImageUrl, string ObjectKey, DateTime ExpiresAt);
