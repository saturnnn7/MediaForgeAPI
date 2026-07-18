using MassTransit;
using MediaForge.Catalog.Application.DTOs;
using MediaForge.Shared.Contracts.Events.Catalog;

namespace MediaForge.Catalog.Application.Commands.UpdatePerson;

public sealed class UpdatePersonCommandHandler(
    IPersonRepository personRepository,
    ICatalogUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<UpdatePersonCommand, Result<PersonDto>>
{
    public async Task<Result<PersonDto>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result.Failure<PersonDto>(Error.NotFound("Person", request.PersonId));

        var updateResult = person.UpdateProfile(request.Name, request.Bio, request.PhotoUrl);
        if (updateResult.IsFailure)
            return Result.Failure<PersonDto>(updateResult.Error);

        personRepository.Update(person);

        await publishEndpoint.Publish(
            new PersonUpdatedEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), person.Id, person.Name, person.Bio, person.PhotoUrl),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(person.ToDto());
    }
}
