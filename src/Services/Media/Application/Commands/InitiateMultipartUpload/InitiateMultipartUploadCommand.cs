using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.InitiateMultipartUpload;

public sealed record InitiateMultipartUploadCommand(string FileName, string ContentType, long FileSizeBytes) : IRequest<Result<MultipartUploadInitiatedDto>>;
