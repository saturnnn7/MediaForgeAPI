namespace MediaForge.Search.Application.DTOs;

public sealed record SearchResultDto(
    IReadOnlyList<MediaDocument> Items,
    long TotalCount,
    int Page,
    int PageSize);
