using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.FindOrCreateByGoogle;

public sealed record FindOrCreateByGoogleCommand(
    string Email,
    string GoogleId,
    string DisplayName,
    string? PhotoUrl) : IRequest<Result<TokensDto>>;
