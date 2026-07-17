using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Abstractions;

public interface IMediaProcessingClient
{
    Task<IReadOnlyList<SuggestedChapterDto>> GetSuggestedChaptersAsync(
        string bucketName, string objectKey, CancellationToken ct);
}
