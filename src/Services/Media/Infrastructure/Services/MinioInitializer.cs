using Amazon.S3;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaForge.Media.Infrastructure.Services;

public sealed class MinioInitializer(IAmazonS3 s3, ILogger<MinioInitializer> logger) : IHostedService
{
    private static readonly string[] Buckets = ["mediaforge-media", "mediaforge-processed"];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var bucket in Buckets)
        {
            try
            {
                var exists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3, bucket);
                if (!exists)
                {
                    await s3.PutBucketAsync(bucket, cancellationToken);
                    logger.LogInformation("Created MinIO bucket: {Bucket}", bucket);
                }
                else
                {
                    logger.LogInformation("MinIO bucket already exists: {Bucket}", bucket);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Could not ensure bucket {Bucket}: {Error}", bucket, ex.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
