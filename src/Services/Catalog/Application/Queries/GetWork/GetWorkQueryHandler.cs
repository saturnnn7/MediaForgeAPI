using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWork;

public sealed class GetWorkQueryHandler(
    IWorkRepository workRepository,
    IGenreRepository genreRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkQuery, Result<WorkDetailDto>>
{
    public async Task<Result<WorkDetailDto>> Handle(GetWorkQuery request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<WorkDetailDto>(Error.NotFound("Work", request.WorkId));

        if (work.IsPrivate
            && !(currentUserService.IsAuthenticated && currentUserService.UserId == work.ChannelId))
        {
            return Result.Failure<WorkDetailDto>(Error.Unauthorized("This content is private."));
        }

        var contributorRows = await workRepository.GetContributorsWithPersonsAsync(request.WorkId, cancellationToken);
        var contributors = contributorRows
            .Select(x => new ContributorDto(x.Person.Id, x.Person.Name, x.Person.PhotoUrl, x.Contributor.Role.ToString(), x.Contributor.DisplayOrder))
            .ToList();

        var genreDtos = new List<GenreDto>();
        foreach (var workGenre in work.Genres)
        {
            var genre = await genreRepository.GetByIdAsync(workGenre.GenreId, cancellationToken);
            if (genre is not null)
                genreDtos.Add(genre.ToDto());
        }

        return Result.Success(work.ToDetailDto(contributors, genreDtos));
    }
}
