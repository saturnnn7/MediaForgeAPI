namespace MediaForge.Processing.Worker.Services;

public interface IStorageService
{
    Task DownloadToFileAsync(string bucketName, string objectKey, string localPath, CancellationToken ct);
    Task<string> UploadFileAsync(string bucketName, string objectKey, string localPath, string contentType, CancellationToken ct);
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct);
}
