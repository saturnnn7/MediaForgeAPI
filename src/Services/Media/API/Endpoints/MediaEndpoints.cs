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
        });

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
}
