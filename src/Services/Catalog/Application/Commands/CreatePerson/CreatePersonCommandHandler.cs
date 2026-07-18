using MassTransit;
using MediaForge.Catalog.Application.DTOs;
using MediaForge.Shared.Contracts.Events.Catalog;

namespace MediaForge.Catalog.Application.Commands.CreatePerson;

public sealed class CreatePersonCommandHandler(
    IPersonRepository personRepository,
    ICatalogUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreatePersonCommand, Result<PersonDto>>
{
    public async Task<Result<PersonDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var personResult = Person.Create(request.Name);
        if (personResult.IsFailure)
            return Result.Failure<PersonDto>(personResult.Error);

        var person = personResult.Value;

        if (request.Bio is not null || request.PhotoUrl is not null)
        {
            var updateResult = person.UpdateProfile(request.Name, request.Bio, request.PhotoUrl);
            if (updateResult.IsFailure)
                return Result.Failure<PersonDto>(updateResult.Error);
        }

        await personRepository.AddAsync(person, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new PersonCreatedEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), person.Id, person.Name, person.Bio, person.PhotoUrl),
            cancellationToken);

        return Result.Success(person.ToDto());
    }
}
