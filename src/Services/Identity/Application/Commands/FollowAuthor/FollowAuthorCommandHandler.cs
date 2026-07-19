using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.FollowAuthor;

public sealed class FollowAuthorCommandHandler(
    ICurrentUserService currentUser,
    IAuthorFollowRepository authorFollowRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<FollowAuthorCommand, Result<AuthorFollowDto>>
{
    public async Task<Result<AuthorFollowDto>> Handle(FollowAuthorCommand request, CancellationToken cancellationToken)
    {
        var existing = await authorFollowRepository.GetAsync(currentUser.UserId, request.PersonId, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<AuthorFollowDto>(Error.Conflict("AuthorFollow", "You are already following this author."));
        }

        var follow = AuthorFollow.Create(currentUser.UserId, request.PersonId);
        await authorFollowRepository.AddAsync(follow, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthorFollowDto(follow.Id, follow.UserId, follow.PersonId, follow.FollowedAt));
    }
}
