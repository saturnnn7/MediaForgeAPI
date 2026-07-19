namespace MediaForge.Library.Application.DTOs;

public sealed record AdminReportDto(
    Guid Id,
    Guid TargetId,
    string TargetType,
    Guid ReporterId,
    string Reason,
    bool IsResolved,
    DateTime? ResolvedAt,
    DateTime CreatedAt);
