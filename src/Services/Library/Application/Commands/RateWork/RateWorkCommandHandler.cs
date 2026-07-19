using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.RateWork;

public sealed class RateWorkCommandHandler(
    ILibraryEntryRepository libraryEntryRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<RateWorkCommand, Result<LibraryEntryDto>>
{
    public async Task<Result<LibraryEntryDto>> Handle(RateWorkCommand request, CancellationToken cancellationToken)
    {
        var entry = await libraryEntryRepository.GetByUserAndWorkAsync(currentUserService.UserId, request.WorkId, cancellationToken);
        if (entry is null)
            return Result.Failure<LibraryEntryDto>(Error.NotFound("LibraryEntry", request.WorkId));

        var rateResult = entry.SetRating(request.Rating);
        if (rateResult.IsFailure)
            return Result.Failure<LibraryEntryDto>(rateResult.Error);

        libraryEntryRepository.Update(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.ToDto());
    }
}
