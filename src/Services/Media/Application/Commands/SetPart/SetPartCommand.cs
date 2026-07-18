namespace MediaForge.Media.Application.Commands.SetPart;

public sealed record SetPartCommand(Guid AssetId, Guid PartId) : IRequest<Result>;
