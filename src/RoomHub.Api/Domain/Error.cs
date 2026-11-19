namespace RoomHub.Api.Domain;

public sealed record Error(string Code, string Detail, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 0);

    public static Error Validation(string code, string detail) => new(code, detail, 400);
    public static Error NotFound(string code, string detail) => new(code, detail, 404);
    public static Error Conflict(string code, string detail) => new(code, detail, 409);
}
