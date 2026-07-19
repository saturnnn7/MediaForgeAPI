namespace MediaForge.Library.Domain.Entities;

public sealed class ListeningProgress
{
    private ListeningProgress() { }

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid EditionId { get; init; }
    public Guid PartId { get; init; }
    public double PositionSeconds { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static ListeningProgress Create(Guid userId, Guid editionId, Guid partId, double positionSeconds) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EditionId = editionId,
            PartId = partId,
            PositionSeconds = positionSeconds,
            UpdatedAt = DateTime.UtcNow
        };

    public void UpdatePosition(double positionSeconds)
    {
        PositionSeconds = positionSeconds;
        UpdatedAt = DateTime.UtcNow;
    }
}
