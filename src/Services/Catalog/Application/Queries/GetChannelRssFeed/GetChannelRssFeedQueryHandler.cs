using MediaForge.Catalog.Application.Abstractions;
using MediaForge.Catalog.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace MediaForge.Catalog.Application.Queries.GetChannelRssFeed;

public sealed class GetChannelRssFeedQueryHandler(
    IWorkRepository workRepository,
    IPartRepository partRepository,
    IConfiguration configuration) : IRequestHandler<GetChannelRssFeedQuery, Result<RssFeedDto>>
{
    public async Task<Result<RssFeedDto>> Handle(GetChannelRssFeedQuery request, CancellationToken cancellationToken)
    {
        var works = await workRepository.GetByChannelIdAsync(request.ChannelId, 1, 100, cancellationToken);
        var publishedWorks = works.Where(w => w.IsPublished && !w.IsPrivate).ToList();

        if (publishedWorks.Count == 0)
            return Result.Failure<RssFeedDto>(Error.NotFound("Channel", request.ChannelId));

        var mediaApiBaseUrl = configuration["MediaApi:BaseUrl"] ?? "http://localhost:5002";
        var items = new List<RssFeedItemDto>();

        foreach (var work in publishedWorks)
        {
            var parts = await partRepository.GetByWorkIdAsync(work.Id, cancellationToken);
            foreach (var part in parts.Where(p => p.IsPublished && !p.IsPrivate))
            {
                var partWithAssets = await partRepository.GetByIdWithDetailsAsync(part.Id, cancellationToken);
                var firstAsset = partWithAssets?.Assets.OrderBy(a => a.SequenceOrder).FirstOrDefault();
                var audioUrl = firstAsset is null ? null : $"{mediaApiBaseUrl}/api/media/{firstAsset.MediaAssetId}/stream";

                items.Add(new RssFeedItemDto(
                    part.Title,
                    part.Description ?? work.Description,
                    part.Id.ToString(),
                    work.PublishedAt ?? work.CreatedAt,
                    audioUrl,
                    part.DurationSeconds,
                    part.OrderMajor,
                    part.OrderMinor));
            }
        }

        var channelTitle = $"MediaForge Channel {request.ChannelId}";
        var feed = new RssFeedDto(
            channelTitle,
            publishedWorks.First().Description,
            publishedWorks.First().CoverUrl,
            $"/api/channels/{request.ChannelId}",
            DateTime.UtcNow,
            items);

        return Result.Success(feed);
    }
}
