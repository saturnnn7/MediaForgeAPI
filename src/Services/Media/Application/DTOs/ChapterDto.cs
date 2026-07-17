namespace MediaForge.Media.Application.DTOs;

public sealed record ChapterDto(Guid Id, string Title, double StartTimeSeconds, double? EndTimeSeconds, int Order);
