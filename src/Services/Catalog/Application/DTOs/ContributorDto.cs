namespace MediaForge.Catalog.Application.DTOs;

public sealed record ContributorDto(Guid PersonId, string Name, string? PhotoUrl, string Role, int DisplayOrder);
