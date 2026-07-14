namespace MediaForge.Shared.Infrastructure.Common;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided.");

    public static Error NotFound(string resource, Guid id) =>
        new($"{resource}.NotFound", $"{resource} with id '{id}' was not found.");

    public static Error Conflict(string resource, string detail) =>
        new($"{resource}.Conflict", detail);

    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);

    public static Error Unauthorized(string message = "Unauthorized access.") =>
        new("Error.Unauthorized", message);
}
