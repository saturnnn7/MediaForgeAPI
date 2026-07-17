namespace MediaForge.Media.Application.Commands.AbortMultipartUpload;

public sealed record AbortMultipartUploadCommand(Guid AssetId, string UploadId) : IRequest<Result>;
