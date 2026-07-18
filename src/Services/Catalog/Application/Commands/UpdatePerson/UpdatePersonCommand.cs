using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdatePerson;

public sealed record UpdatePersonCommand(Guid PersonId, string Name, string? Bio, string? PhotoUrl) : IRequest<Result<PersonDto>>;
