namespace MediaForge.Catalog.Domain.Entities;

public sealed class PartAsset
{
    private PartAsset() { }

    public Guid Id { get; init; }
    public Guid PartId { get; init; }
    public Guid MediaAssetId { get; init; }
    public int SequenceOrder { get; private set; }

    public static PartAsset Create(Guid partId, Guid mediaAssetId, int sequenceOrder) =>
        new()
        {
            Id = Guid.NewGuid(),
            PartId = partId,
            MediaAssetId = mediaAssetId,
            SequenceOrder = sequenceOrder
        };
}
