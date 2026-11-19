namespace RoomHub.Api.Domain;

public interface IRoomRepository
{
    Task AddAsync(Room room, CancellationToken ct = default);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
