namespace MediaForge.Catalog.Application.DTOs;

public sealed record WorkRequestDto(
    Guid Id,
    Guid RequesterId,
    string WorkType,
    string Title,
    string? AuthorNames,
    string? Description,
    string? CoverUrl,
    string? Language,
    string Status,
    string? AdminNote,
    Guid? ResultingWorkId,
    DateTime CreatedAt,
    DateTime? ReviewedAt);
