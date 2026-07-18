using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.AddChapterToPart;

public sealed record AddChapterToPartCommand(
    Guid PartId,
    string Title,
    double StartTimeSeconds,
    double EndTimeSeconds,
    int Order) : IRequest<Result<PartChapterDto>>;
