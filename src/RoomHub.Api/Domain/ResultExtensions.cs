namespace RoomHub.Api.Domain;

public static class ResultExtensions
{
    public static IResult ToProblem(this Error error) =>
        Results.Problem(
            detail: error.Detail,
            statusCode: error.StatusCode,
            title: error.Code);
}
