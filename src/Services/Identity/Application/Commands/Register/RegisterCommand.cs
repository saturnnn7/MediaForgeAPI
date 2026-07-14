using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string DisplayName) : IRequest<Result<UserProfileDto>>;
