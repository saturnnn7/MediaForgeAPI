namespace MediaForge.Library.Application.DTOs;

public static class LibraryEntryMapper
{
    public static LibraryEntryDto ToDto(this LibraryEntry entry) =>
        new(
            entry.Id,
            entry.UserId,
            entry.WorkId,
            entry.Status.ToString(),
            entry.IsFavorite,
            entry.Rating,
            entry.Privacy.ToString(),
            entry.CreatedAt,
            entry.UpdatedAt);
}
