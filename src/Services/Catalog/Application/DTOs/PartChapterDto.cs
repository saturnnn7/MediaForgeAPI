namespace MediaForge.Catalog.Application.DTOs;

public sealed record PartChapterDto(Guid Id, string Title, double StartTimeSeconds, double EndTimeSeconds, int Order);
