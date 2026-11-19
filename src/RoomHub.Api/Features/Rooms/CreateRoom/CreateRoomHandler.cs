using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int Capacity);

public sealed record CreateRoomResult(Guid Id, string Name, int Capacity);

public sealed class CreateRoomHandler(IRoomRepository rooms, IUnitOfWork uow)
    : ICommandHandler<CreateRoomCommand, CreateRoomResult>
{
    public async Task<Result<CreateRoomResult>> HandleAsync(
        CreateRoomCommand command, CancellationToken ct = default)
    {
        var result = Room.Create(command.Name, command.Capacity);
        if (result.IsFailure)
            return result.Error;

        var room = result.Value;
        await rooms.AddAsync(room, ct);
        await uow.SaveChangesAsync(ct);

        return new CreateRoomResult(room.Id, room.Name, room.Capacity);
    }
}
