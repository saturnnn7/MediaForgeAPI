namespace MediaForge.Catalog.Domain.Entities;

public sealed class Person : AggregateRoot
{
    private const int MaxNameLength = 200;
    private const int MaxBioLength = 2000;
    private const int MaxPhotoUrlLength = 2000;

    private Person() { }

    public string Name { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public string? PhotoUrl { get; private set; }
    public DateTime CreatedAt { get; init; }

    public static Result<Person> Create(string name)
    {
        var validation = Validate(name);
        if (validation.IsFailure)
            return Result.Failure<Person>(validation.Error);

        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(person);
    }

    public Result UpdateProfile(string name, string? bio, string? photoUrl)
    {
        var validation = Validate(name, bio, photoUrl);
        if (validation.IsFailure)
            return validation;

        Name = name;
        Bio = bio;
        PhotoUrl = photoUrl;

        return Result.Success();
    }

    private static Result Validate(string name, string? bio = null, string? photoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Name", "Name is required."));

        if (name.Length > MaxNameLength)
            return Result.Failure(Error.Validation("Name", $"Name must not exceed {MaxNameLength} characters."));

        if (bio is { Length: > MaxBioLength })
            return Result.Failure(Error.Validation("Bio", $"Bio must not exceed {MaxBioLength} characters."));

        if (photoUrl is { Length: > MaxPhotoUrlLength })
            return Result.Failure(Error.Validation("PhotoUrl", $"PhotoUrl must not exceed {MaxPhotoUrlLength} characters."));

        return Result.Success();
    }
}
