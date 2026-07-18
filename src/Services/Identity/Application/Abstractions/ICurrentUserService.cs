namespace MediaForge.Identity.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}
