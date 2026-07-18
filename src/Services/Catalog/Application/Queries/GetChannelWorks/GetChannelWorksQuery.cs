using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetChannelWorks;

public sealed record GetChannelWorksQuery(Guid ChannelId, int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<WorkSummaryDto>>>;
