using MediaForge.Library.Application.Commands.AddComment;
using MediaForge.Library.Application.Commands.CreateReview;
using MediaForge.Library.Application.Commands.EditReview;
using MediaForge.Library.Application.Commands.ReportContent;
using MediaForge.Library.Application.Commands.ToggleReaction;
using MediaForge.Library.Application.Queries.GetWorkReviews;
using MediaForge.Library.Domain.Enums;

namespace MediaForge.Library.API.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/reviews", async (CreateReviewRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateReviewCommand(request.WorkId, request.Text, request.ContainsSpoiler);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/reviews/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        })
        .RequireAuthorization()
        .WithSummary("Create a review for a work")
        .WithDescription("Text supports spoiler syntax (||text||) when ContainsSpoiler is true.");

        app.MapPut("/api/reviews/{reviewId:guid}", async (Guid reviewId, EditReviewRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new EditReviewCommand(reviewId, request.Text, request.ContainsSpoiler), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/works/{workId:guid}/reviews", async (Guid workId, int? page, int? pageSize, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkReviewsQuery(workId, page ?? 1, pageSize ?? 20), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/reviews/{reviewId:guid}/comments", async (Guid reviewId, AddCommentRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddCommentCommand(reviewId, request.ParentCommentId, request.Text, request.ContainsSpoiler);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/reviews/{reviewId}/comments/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/reactions", async (ToggleReactionRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ToggleReactionCommand(request.TargetId, request.TargetType, request.Emoji), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/reports", async (ReportContentRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ReportContentCommand(request.TargetId, request.TargetType, request.Reason), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record CreateReviewRequest(Guid WorkId, string Text, bool ContainsSpoiler);

    private sealed record EditReviewRequest(string Text, bool ContainsSpoiler);

    private sealed record AddCommentRequest(Guid? ParentCommentId, string Text, bool ContainsSpoiler);

    private sealed record ToggleReactionRequest(Guid TargetId, ReactionTarget TargetType, string Emoji);

    private sealed record ReportContentRequest(Guid TargetId, ReactionTarget TargetType, string Reason);
}
