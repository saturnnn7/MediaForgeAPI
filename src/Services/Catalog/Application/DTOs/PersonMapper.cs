namespace MediaForge.Catalog.Application.DTOs;

public static class PersonMapper
{
    public static PersonDto ToDto(this Person person) =>
        new(person.Id, person.Name, person.Bio, person.PhotoUrl, person.CreatedAt);
}
