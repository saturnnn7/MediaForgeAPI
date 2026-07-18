namespace MediaForge.Catalog.Domain.Entities;

public sealed class Genre : AggregateRoot
{
    private const int MaxNameLength = 100;
    private const int MaxSlugLength = 100;
    private const int MaxDescriptionLength = 500;

    private Genre() { }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static Result<Genre> Create(string name, string slug)
    {
        var validation = Validate(name, slug);
        if (validation.IsFailure)
            return Result.Failure<Genre>(validation.Error);

        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug
        };

        return Result.Success(genre);
    }

    public Result Update(string name, string? description)
    {
        var validation = Validate(name, Slug, description);
        if (validation.IsFailure)
            return validation;

        Name = name;
        Description = description;

        return Result.Success();
    }

    private static Result Validate(string name, string slug, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Name", "Name is required."));

        if (name.Length > MaxNameLength)
            return Result.Failure(Error.Validation("Name", $"Name must not exceed {MaxNameLength} characters."));

        if (string.IsNullOrWhiteSpace(slug))
            return Result.Failure(Error.Validation("Slug", "Slug is required."));

        if (slug.Length > MaxSlugLength)
            return Result.Failure(Error.Validation("Slug", $"Slug must not exceed {MaxSlugLength} characters."));

        if (description is { Length: > MaxDescriptionLength })
            return Result.Failure(Error.Validation("Description", $"Description must not exceed {MaxDescriptionLength} characters."));

        return Result.Success();
    }
}
