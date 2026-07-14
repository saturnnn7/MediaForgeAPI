namespace MediaForge.Media.Application.DTOs;

public sealed record UserDto(Guid Id, string Email, string DisplayName, bool IsEmailVerified);
