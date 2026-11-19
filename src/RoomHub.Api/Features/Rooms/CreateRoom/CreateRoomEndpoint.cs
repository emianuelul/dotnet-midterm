using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomRequest(string? Name, int Capacity);

public static class CreateRoomEndpoint
{
    public static RouteGroupBuilder MapCreateRoom(this RouteGroupBuilder group, string endpointName = "CreateRoom")
    {
        group.MapPost("/", async (
            CreateRoomRequest request,
            ICommandHandler<CreateRoomCommand, CreateRoomResult> handler,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new CreateRoomCommand(request.Name, request.Capacity), ct);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var room = result.Value;
            var path = httpContext.Request.Path.Value?.TrimEnd('/');
            return Results.Created(
                $"{path}/{room.Id}",
                new { room.Id, room.Name, room.Capacity });
        })
        .WithName(endpointName)
        .WithSummary("Create a new room");

        return group;
    }
}
