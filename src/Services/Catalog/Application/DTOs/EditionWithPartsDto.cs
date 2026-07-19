namespace MediaForge.Catalog.Application.DTOs;

public sealed record EditionWithPartsDto(
    Guid Id,
    Guid WorkId,
    string NarratorTeamName,
    string Language,
    bool IsDefault,
    IReadOnlyList<PartSummaryDto> Parts);
