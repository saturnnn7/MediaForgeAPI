using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetUserProfile;

public sealed record GetUserProfileQuery : IRequest<Result<UserProfileDto>>;
