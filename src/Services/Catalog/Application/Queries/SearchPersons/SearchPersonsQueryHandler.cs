using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.SearchPersons;

public sealed class SearchPersonsQueryHandler(IPersonRepository personRepository)
    : IRequestHandler<SearchPersonsQuery, Result<IReadOnlyList<PersonDto>>>
{
    public async Task<Result<IReadOnlyList<PersonDto>>> Handle(SearchPersonsQuery request, CancellationToken cancellationToken)
    {
        var persons = await personRepository.SearchAsync(request.Query, request.Page, request.PageSize, cancellationToken);
        return Result.Success<IReadOnlyList<PersonDto>>(persons.Select(p => p.ToDto()).ToList());
    }
}
