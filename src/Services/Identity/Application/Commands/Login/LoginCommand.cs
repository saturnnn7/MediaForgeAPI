using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<TokensDto>>;
