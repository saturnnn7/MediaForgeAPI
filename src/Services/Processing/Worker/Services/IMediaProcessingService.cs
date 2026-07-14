using MediaForge.Processing.Worker.Models;

namespace MediaForge.Processing.Worker.Services;

public interface IMediaProcessingService
{
    Task<ProcessingResult> ProcessAsync(
        string localFilePath,
        string contentType,
        string originalFileName,
        string outputBucketName,
        string assetId,
        CancellationToken ct);
}
