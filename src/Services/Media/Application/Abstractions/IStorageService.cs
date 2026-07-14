namespace MediaForge.Media.Application.Abstractions;

public interface IStorageService
{
    Task<string> GenerateUploadUrlAsync(string bucketName, string objectKey, string contentType, TimeSpan expiry, CancellationToken ct);
    Task<string> GenerateDownloadUrlAsync(string bucketName, string objectKey, TimeSpan expiry, CancellationToken ct);
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct);
}
