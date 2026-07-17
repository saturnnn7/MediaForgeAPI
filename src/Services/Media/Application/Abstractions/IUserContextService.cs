namespace MediaForge.Media.Application.Abstractions;

public interface IUserContextService
{
    Guid UserId { get; }
    string Email { get; }
    string Role { get; }
}
