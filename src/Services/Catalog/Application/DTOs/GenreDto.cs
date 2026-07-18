namespace MediaForge.Catalog.Application.DTOs;

public sealed record GenreDto(Guid Id, string Name, string Slug, string? Description);
