namespace MediaForge.Catalog.Domain.Entities;

public sealed class WorkContributor
{
    private WorkContributor() { }

    public Guid Id { get; init; }
    public Guid WorkId { get; init; }
    public Guid PersonId { get; init; }
    public ContributorRole Role { get; init; }
    public int DisplayOrder { get; private set; }

    public static WorkContributor Create(Guid workId, Guid personId, ContributorRole role) =>
        new()
        {
            Id = Guid.NewGuid(),
            WorkId = workId,
            PersonId = personId,
            Role = role,
            DisplayOrder = 0
        };
}
