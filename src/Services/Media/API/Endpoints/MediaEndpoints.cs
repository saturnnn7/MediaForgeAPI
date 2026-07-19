namespace MediaForge.Media.API.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/media").RequireAuthorization();

        group.MapPost("/upload-url", async (RequestUploadUrlCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/media/{result.Value.AssetId}", result.Value)
                : ToHttpResult(result);
        })
        .WithSummary("Request a presigned upload URL")
        .WithDescription("Creates a pending media asset and returns a presigned S3 URL for direct client upload.");

        group.MapPost("/{assetId:guid}/confirm-upload", async (Guid assetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ConfirmUploadCommand(assetId), ct);
            return result.IsSuccess
                ? Results.Accepted()
                : ToHttpResult(result);
        });

        group.MapGet("/{assetId:guid}", async (Guid assetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMediaAssetQuery(assetId), ct);
            return ToHttpResult(result);
        });

        group.MapGet("/", async (ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new GetUserMediaAssetsQuery(page, Math.Min(pageSize, 50)), ct);
            return ToHttpResult(result);
        });

        app.MapGet("/api/media/{assetId:guid}/stream", async (Guid assetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetStreamingUrlsQuery(assetId), ct);
            return ToHttpResult(result);
        });

        group.MapPost("/multipart/initiate", async (InitiateMultipartUploadCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/media/{result.Value.AssetId}", result.Value)
                : ToHttpResult(result);
        });

        group.MapPost("/multipart/part-url", async (GeneratePartUrlCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { url = result.Value })
                : ToHttpResult(result);
        });

        group.MapPost("/multipart/complete", async (CompleteMultipartUploadCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Accepted(value: result.Value)
                : ToHttpResult(result);
        });

        group.MapPost("/multipart/abort", async (AbortMultipartUploadCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.NoContent()
                : ToHttpResult(result);
        });

        app.MapGet("/api/media/{assetId:guid}/chapters", async (Guid assetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetChaptersQuery(assetId), ct);
            return ToHttpResult(result);
        });

        group.MapPost("/{assetId:guid}/chapters", async (Guid assetId, AddChapterRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddChapterCommand(assetId, request.Title, request.StartTimeSeconds, request.Order, request.EndTimeSeconds);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/media/{assetId}/chapters", result.Value)
                : ToHttpResult(result);
        });

        group.MapPut("/{assetId:guid}/chapters/{chapterId:guid}", async (Guid assetId, Guid chapterId, UpdateChapterRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateChapterCommand(assetId, chapterId, request.Title, request.StartTimeSeconds, request.EndTimeSeconds);
            var result = await sender.Send(command, ct);
            return ToHttpResult(result);
        });

        group.MapDelete("/{assetId:guid}/chapters/{chapterId:guid}", async (Guid assetId, Guid chapterId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteChapterCommand(assetId, chapterId), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : ToHttpResult(result);
        });

        group.MapGet("/{assetId:guid}/chapters/suggestions", async (Guid assetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSuggestedChaptersQuery(assetId), ct);
            return ToHttpResult(result);
        });

        group.MapPatch("/{assetId:guid}/part", async (Guid assetId, SetPartRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetPartCommand(assetId, request.PartId), ct);
            return ToHttpResult(result);
        });

        group.MapPost("/images/upload-url", async (RequestImageUploadUrlCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created(result.Value.ImageUrl, result.Value)
                : ToHttpResult(result);
        });

        return app;
    }

    private static IResult ToHttpResult<T>(Result<T> result) => result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error.Code.Contains("NotFound") ? Results.NotFound(result.Error)
        : result.Error.Code.Contains("Unauthorized") ? Results.Unauthorized()
        : Results.BadRequest(result.Error);

    private static IResult ToHttpResult(Result result) => result.IsSuccess
        ? Results.Ok()
        : result.Error.Code.Contains("NotFound") ? Results.NotFound(result.Error)
        : result.Error.Code.Contains("Unauthorized") ? Results.Unauthorized()
        : Results.BadRequest(result.Error);

    private sealed record AddChapterRequest(string Title, double StartTimeSeconds, int Order, double? EndTimeSeconds);

    private sealed record UpdateChapterRequest(string Title, double StartTimeSeconds, double? EndTimeSeconds);

    private sealed record SetPartRequest(Guid PartId);
}
