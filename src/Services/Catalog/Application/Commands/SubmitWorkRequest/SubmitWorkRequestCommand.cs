using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.SubmitWorkRequest;

public sealed record SubmitWorkRequestCommand(
    WorkType WorkType,
    string Title,
    string? AuthorNames,
    string? Description,
    string? CoverUrl,
    string? Language) : IRequest<Result<WorkRequestDto>>;
