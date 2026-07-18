using MediaForge.Catalog.Application.Commands.CreatePerson;
using MediaForge.Catalog.Application.Commands.UpdatePerson;
using MediaForge.Catalog.Application.Queries.GetPerson;
using MediaForge.Catalog.Application.Queries.SearchPersons;

namespace MediaForge.Catalog.API.Endpoints;

public static class PersonEndpoints
{
    public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/persons");

        group.MapGet("/{personId:guid}", async (Guid personId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPersonQuery(personId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapGet("/", async (ISender sender, CancellationToken ct, string q = "", int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new SearchPersonsQuery(q, page, Math.Min(pageSize, 50)), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapPost("/", async (CreatePersonCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/persons/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        group.MapPut("/{personId:guid}", async (Guid personId, UpdatePersonRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdatePersonCommand(personId, request.Name, request.Bio, request.PhotoUrl);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record UpdatePersonRequest(string Name, string? Bio, string? PhotoUrl);
}
