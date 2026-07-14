using Amazon.S3;
using Amazon.S3.Model;

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

    public Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct)
        => Task.CompletedTask;
}
