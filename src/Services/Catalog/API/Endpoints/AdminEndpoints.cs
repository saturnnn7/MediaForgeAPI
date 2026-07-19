using MediaForge.Catalog.Application.Queries.GetAdminWorkRequests;
using MediaForge.Catalog.Domain.Enums;

namespace MediaForge.Catalog.API.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/work-requests", async (string? status, int? page, int? pageSize, ISender sender, CancellationToken ct) =>
        {
            WorkRequestStatus? statusFilter = status is not null && Enum.TryParse<WorkRequestStatus>(status, true, out var parsed) ? parsed : null;
            var result = await sender.Send(new GetAdminWorkRequestsQuery(statusFilter, page ?? 1, pageSize ?? 20), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }
}
