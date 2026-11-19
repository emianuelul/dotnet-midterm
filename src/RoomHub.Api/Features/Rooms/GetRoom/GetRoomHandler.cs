using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomQuery(Guid Id);

public sealed record GetRoomResult(Guid Id, string Name, int Capacity);

public sealed class GetRoomHandler(IRoomRepository rooms)
    : IQueryHandler<GetRoomQuery, GetRoomResult>
{
    public async Task<Result<GetRoomResult>> HandleAsync(
        GetRoomQuery query, CancellationToken ct = default)
    {
        var room = await rooms.GetByIdAsync(query.Id, ct);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        return new GetRoomResult(room.Id, room.Name, room.Capacity);
    }
}
