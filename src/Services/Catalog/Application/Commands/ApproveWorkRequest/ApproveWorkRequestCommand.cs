using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.ApproveWorkRequest;

public sealed record ApproveWorkRequestCommand(Guid RequestId, Guid ChannelId) : IRequest<Result<WorkRequestDto>>;
