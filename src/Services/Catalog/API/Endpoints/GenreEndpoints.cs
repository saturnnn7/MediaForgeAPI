using MediaForge.Catalog.Application.Commands.CreateGenre;
using MediaForge.Catalog.Application.Commands.UpdateGenre;
using MediaForge.Catalog.Application.Queries.GetAllGenres;
using MediaForge.Catalog.Application.Queries.GetGenre;

namespace MediaForge.Catalog.API.Endpoints;

public static class GenreEndpoints
{
    public static IEndpointRouteBuilder MapGenreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/genres");

        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllGenresQuery(), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapGet("/{genreId:guid}", async (Guid genreId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetGenreQuery(genreId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapPost("/", async (CreateGenreCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/genres/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        group.MapPut("/{genreId:guid}", async (Guid genreId, UpdateGenreRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateGenreCommand(genreId, request.Name, request.Description);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record UpdateGenreRequest(string Name, string? Description);
}
