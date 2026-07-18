namespace MediaForge.Catalog.Application.DTOs;

public sealed record PersonDto(Guid Id, string Name, string? Bio, string? PhotoUrl, DateTime CreatedAt);
