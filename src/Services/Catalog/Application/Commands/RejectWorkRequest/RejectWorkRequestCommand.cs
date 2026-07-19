using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.RejectWorkRequest;

public sealed record RejectWorkRequestCommand(Guid RequestId, string? AdminNote) : IRequest<Result<WorkRequestDto>>;
