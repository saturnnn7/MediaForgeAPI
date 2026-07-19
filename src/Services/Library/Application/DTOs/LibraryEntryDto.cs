namespace MediaForge.Library.Application.DTOs;

public sealed record LibraryEntryDto(
    Guid Id,
    Guid UserId,
    Guid WorkId,
    string Status,
    bool IsFavorite,
    int? Rating,
    string Privacy,
    DateTime CreatedAt,
    DateTime UpdatedAt);
