using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.AddChapterToPart;

public sealed class AddChapterToPartCommandHandler(
    IPartRepository partRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<AddChapterToPartCommand, Result<PartChapterDto>>
{
    public async Task<Result<PartChapterDto>> Handle(AddChapterToPartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdWithDetailsAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure<PartChapterDto>(Error.NotFound("Part", request.PartId));

        var addResult = part.AddChapter(request.Title, request.StartTimeSeconds, request.EndTimeSeconds, request.Order);
        if (addResult.IsFailure)
            return Result.Failure<PartChapterDto>(addResult.Error);

        await partRepository.AddChapterAsync(addResult.Value, cancellationToken);
        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(addResult.Value.ToDto());
    }
}
