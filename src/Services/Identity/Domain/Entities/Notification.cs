using System.Text.Json;

namespace MediaForge.Identity.Domain.Entities;

public sealed class Notification
{
    private Notification() { }

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public NotificationType Type { get; init; }
    public string Payload { get; init; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; init; }

    public static Notification Create(Guid userId, NotificationType type, object payload) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Payload = JsonSerializer.Serialize(payload),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

    public void MarkAsRead() => IsRead = true;
}
