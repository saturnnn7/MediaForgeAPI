using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetPerson;

public sealed class GetPersonQueryHandler(IPersonRepository personRepository)
    : IRequestHandler<GetPersonQuery, Result<PersonDto>>
{
    public async Task<Result<PersonDto>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        return person is null
            ? Result.Failure<PersonDto>(Error.NotFound("Person", request.PersonId))
            : Result.Success(person.ToDto());
    }
}
