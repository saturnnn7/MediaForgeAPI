using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetMyLibrary;

public sealed record GetMyLibraryQuery : IRequest<Result<IReadOnlyList<LibraryEntryDto>>>;
