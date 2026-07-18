namespace MediaForge.Search.Application.DTOs;

public sealed record SearchResultDto<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int Page,
    int PageSize);
