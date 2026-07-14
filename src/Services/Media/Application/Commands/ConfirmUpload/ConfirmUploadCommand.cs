namespace MediaForge.Media.Application.Commands.ConfirmUpload;

public sealed record ConfirmUploadCommand(Guid AssetId) : IRequest<Result>;
