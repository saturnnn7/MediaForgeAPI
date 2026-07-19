namespace MediaForge.Library.Application.DTOs;

public static class ListeningProgressMapper
{
    public static ListeningProgressDto ToDto(this ListeningProgress progress) =>
        new(
            progress.Id,
            progress.UserId,
            progress.EditionId,
            progress.PartId,
            progress.PositionSeconds,
            progress.UpdatedAt);
}
