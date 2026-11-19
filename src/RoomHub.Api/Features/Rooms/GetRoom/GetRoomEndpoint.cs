using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public static RouteGroupBuilder MapGetRoom(this RouteGroupBuilder group, string endpointName = "GetRoomById")
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, GetRoomResult> handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetRoomQuery(id), ct);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var room = result.Value;
            return Results.Ok(new { room.Id, room.Name, room.Capacity });
        })
        .WithName(endpointName)
        .WithSummary("Get a room by ID");

        return group;
    }
}
