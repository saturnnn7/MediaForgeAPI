namespace MediaForge.Identity.Application.DTOs;

public sealed record TokensDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
