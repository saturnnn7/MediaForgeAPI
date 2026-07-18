using MassTransit;
using MediaForge.Shared.Contracts.Events.Catalog;

namespace MediaForge.Catalog.Application.Commands.PublishWork;

public sealed class PublishWorkCommandHandler(
    IWorkRepository workRepository,
    IGenreRepository genreRepository,
    ICatalogUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<PublishWorkCommand, Result>
{
    public async Task<Result> Handle(PublishWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        var publishResult = work.Publish();
        if (publishResult.IsFailure)
            return publishResult;

        workRepository.Update(work);

        var contributors = await workRepository.GetContributorsWithPersonsAsync(work.Id, cancellationToken);
        var contributorNames = contributors.Select(x => x.Person.Name).ToList();

        var genreNames = new List<string>();
        foreach (var workGenre in work.Genres)
        {
            var genre = await genreRepository.GetByIdAsync(workGenre.GenreId, cancellationToken);
            if (genre is not null)
                genreNames.Add(genre.Name);
        }

        await publishEndpoint.Publish(
            new WorkPublishedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                Guid.NewGuid(),
                work.Id,
                work.ChannelId,
                work.SeriesId,
                work.Title,
                work.Description,
                work.WorkType.ToString(),
                work.Language,
                work.CoverUrl,
                contributorNames,
                genreNames),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
