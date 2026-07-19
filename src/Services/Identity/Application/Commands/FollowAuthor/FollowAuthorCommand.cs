using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.FollowAuthor;

public sealed record FollowAuthorCommand(Guid PersonId) : IRequest<Result<AuthorFollowDto>>;
