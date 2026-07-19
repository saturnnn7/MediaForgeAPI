using MediaForge.Library.Application.Commands.HideContent;
using MediaForge.Library.Application.Commands.ResolveReport;
using MediaForge.Library.Application.Queries.GetAdminReports;
using MediaForge.Library.Domain.Enums;

namespace MediaForge.Library.API.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/reports", async (int? page, int? pageSize, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAdminReportsQuery(page ?? 1, pageSize ?? 20), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/admin/reports/{reportId:guid}/resolve", async (Guid reportId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ResolveReportCommand(reportId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/admin/content/{targetId:guid}/hide", async (Guid targetId, HideContentRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new HideContentCommand(targetId, request.TargetType), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record HideContentRequest(ReactionTarget TargetType);
}
