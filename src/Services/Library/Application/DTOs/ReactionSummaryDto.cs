namespace MediaForge.Library.Application.DTOs;

public sealed record ReactionSummaryDto(string Emoji, int Count, bool CurrentUserReacted);
