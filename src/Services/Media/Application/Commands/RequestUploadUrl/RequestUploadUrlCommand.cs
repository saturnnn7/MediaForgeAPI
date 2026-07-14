using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.RequestUploadUrl;

public sealed record RequestUploadUrlCommand(string FileName, string ContentType, long FileSizeBytes) : IRequest<Result<UploadUrlDto>>;
