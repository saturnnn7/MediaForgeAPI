using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokensDto>>;
