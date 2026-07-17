using Amazon.S3;
using Amazon.S3.Model;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Infrastructure.Services;

public sealed class StorageService(IAmazonS3 s3Client) : IStorageService
{
    public Task<string> GenerateUploadUrlAsync(string bucketName, string objectKey, string contentType, TimeSpan expiry, CancellationToken ct)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.Add(expiry)
        };

        var url = s3Client.GetPreSignedURL(request)
            .Replace("https://localhost", "http://localhost", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(url);
    }

    public Task<string> GenerateDownloadUrlAsync(string bucketName, string objectKey, TimeSpan expiry, CancellationToken ct)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiry)
        };

        var url = s3Client.GetPreSignedURL(request)
            .Replace("https://localhost", "http://localhost", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(url);
    }

    public async Task DownloadToFileAsync(string bucketName, string objectKey, string localPath, CancellationToken ct)
    {
        var request = new GetObjectRequest { BucketName = bucketName, Key = objectKey };
        using var response = await s3Client.GetObjectAsync(request, ct);
        using var fs = File.Create(localPath);
        await response.ResponseStream.CopyToAsync(fs, ct);
    }

    public Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct)
        => Task.CompletedTask;

    public async Task<string> InitiateMultipartUploadAsync(string bucketName, string objectKey, string contentType, CancellationToken ct)
    {
        var response = await s3Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            ContentType = contentType
        }, ct);

        return response.UploadId;
    }

    public Task<string> GeneratePartUploadUrlAsync(string bucketName, string objectKey, string uploadId, int partNumber, CancellationToken ct)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(60),
            UploadId = uploadId,
            PartNumber = partNumber
        };

        var url = s3Client.GetPreSignedURL(request)
            .Replace("https://localhost", "http://localhost", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(url);
    }

    public async Task<string> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IReadOnlyList<CompletedPartDto> parts, CancellationToken ct)
    {
        await s3Client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            UploadId = uploadId,
            PartETags = parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        }, ct);

        return $"http://localhost:9000/{bucketName}/{objectKey}";
    }

    public Task AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, CancellationToken ct)
        => s3Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            UploadId = uploadId
        }, ct);
}
