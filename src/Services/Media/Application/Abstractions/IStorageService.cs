using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Abstractions;

public interface IStorageService
{
    Task<string> GenerateUploadUrlAsync(string bucketName, string objectKey, string contentType, TimeSpan expiry, CancellationToken ct);
    Task<string> GenerateDownloadUrlAsync(string bucketName, string objectKey, TimeSpan expiry, CancellationToken ct);
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct);
    Task<string> InitiateMultipartUploadAsync(string bucketName, string objectKey, string contentType, CancellationToken ct);
    Task<string> GeneratePartUploadUrlAsync(string bucketName, string objectKey, string uploadId, int partNumber, CancellationToken ct);
    Task<string> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IReadOnlyList<CompletedPartDto> parts, CancellationToken ct);
    Task AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, CancellationToken ct);
}
