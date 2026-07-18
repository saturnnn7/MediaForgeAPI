namespace MediaForge.Catalog.Application.DTOs;

public static class GenreMapper
{
    public static GenreDto ToDto(this Genre genre) =>
        new(genre.Id, genre.Name, genre.Slug, genre.Description);
}
