using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreatePerson;

public sealed record CreatePersonCommand(string Name, string? Bio, string? PhotoUrl) : IRequest<Result<PersonDto>>;
