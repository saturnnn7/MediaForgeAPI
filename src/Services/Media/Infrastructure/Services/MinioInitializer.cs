using Amazon.S3;
using Amazon.S3.Model;
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

                if (bucket == "mediaforge-processed")
                {
                    logger.LogInformation("Applying public read policy to bucket: {Bucket}", bucket);
                    try
                    {
                        await s3.PutBucketPolicyAsync(new PutBucketPolicyRequest
                        {
                            BucketName = bucket,
                            Policy = """
                            {
                              "Version": "2012-10-17",
                              "Statement": [{
                                "Effect": "Allow",
                                "Principal": {"AWS": ["*"]},
                                "Action": ["s3:GetObject"],
                                "Resource": ["arn:aws:s3:::mediaforge-processed/*"]
                              }]
                            }
                            """
                        }, cancellationToken);
                        logger.LogInformation("Applied public read policy to bucket: {Bucket}", bucket);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to apply public read policy to bucket {Bucket}: {Error}", bucket, ex.Message);
                    }
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
