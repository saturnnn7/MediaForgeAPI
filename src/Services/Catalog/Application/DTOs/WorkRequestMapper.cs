namespace MediaForge.Catalog.Application.DTOs;

public static class WorkRequestMapper
{
    public static WorkRequestDto ToDto(this WorkRequest request) =>
        new(
            request.Id,
            request.RequesterId,
            request.WorkType.ToString(),
            request.Title,
            request.AuthorNames,
            request.Description,
            request.CoverUrl,
            request.Language,
            request.Status.ToString(),
            request.AdminNote,
            request.ResultingWorkId,
            request.CreatedAt,
            request.ReviewedAt);
}
