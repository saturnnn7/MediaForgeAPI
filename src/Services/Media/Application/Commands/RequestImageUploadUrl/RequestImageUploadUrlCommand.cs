using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Commands.RequestImageUploadUrl;

public sealed record RequestImageUploadUrlCommand(string Category, string FileName, string ContentType) : IRequest<Result<ImageUploadUrlDto>>;
