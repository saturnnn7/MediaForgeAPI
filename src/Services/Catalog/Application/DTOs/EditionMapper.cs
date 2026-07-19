namespace MediaForge.Catalog.Application.DTOs;

public static class EditionMapper
{
    public static EditionDto ToDto(this Edition edition) =>
        new(
            edition.Id,
            edition.WorkId,
            edition.CreatorId,
            edition.NarratorTeamName,
            edition.Description,
            edition.CoverUrl,
            edition.Language,
            edition.IsDefault,
            edition.CreatedAt);

    public static EditionWithPartsDto ToWithPartsDto(this Edition edition) =>
        new(
            edition.Id,
            edition.WorkId,
            edition.NarratorTeamName,
            edition.Language,
            edition.IsDefault,
            edition.Parts
                .OrderBy(p => p.OrderMajor).ThenBy(p => p.OrderMinor)
                .Select(p => p.ToSummaryDto(edition.NarratorTeamName))
                .ToList());
}
