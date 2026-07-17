namespace MediaForge.Media.Application.Commands.GeneratePartUrl;

public sealed record GeneratePartUrlCommand(Guid AssetId, string UploadId, int PartNumber) : IRequest<Result<string>>;
