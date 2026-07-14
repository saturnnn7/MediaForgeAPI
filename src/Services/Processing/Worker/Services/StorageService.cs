using Amazon.S3;
using Amazon.S3.Model;

namespace MediaForge.Processing.Worker.Services;

public sealed class StorageService(IAmazonS3 s3Client, IConfiguration configuration) : IStorageService
{
    public async Task DownloadToFileAsync(string bucketName, string objectKey, string localPath, CancellationToken ct)
    {
        var request = new GetObjectRequest { BucketName = bucketName, Key = objectKey };
        using var response = await s3Client.GetObjectAsync(request, ct);
        using var fs = File.Create(localPath);
        await response.ResponseStream.CopyToAsync(fs, ct);
    }

    public async Task<string> UploadFileAsync(string bucketName, string objectKey, string localPath, string contentType, CancellationToken ct)
    {
        using var fs = File.OpenRead(localPath);
        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            InputStream = fs,
            ContentType = contentType
        }, ct);

        var serviceUrl = configuration["Storage:ServiceUrl"];
        return $"{serviceUrl}/{bucketName}/{objectKey}";
    }

    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct)
    {
        try
        {
            await s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = bucketName,
                MaxKeys = 1
            }, ct);
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchBucket")
        {
            await s3Client.PutBucketAsync(bucketName, ct);
        }
    }
}
