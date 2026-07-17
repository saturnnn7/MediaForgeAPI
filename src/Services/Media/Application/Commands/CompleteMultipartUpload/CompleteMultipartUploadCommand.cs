using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.CompleteMultipartUpload;

public sealed record CompleteMultipartUploadCommand(Guid AssetId, string UploadId, IReadOnlyList<CompletedPartDto> Parts) : IRequest<Result<MultipartUploadCompletedDto>>;
