using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class RoomRepository(RoomHubDbContext db) : IRoomRepository
{
    public async Task AddAsync(Room room, CancellationToken ct = default)
    {
        await db.Rooms.AddAsync(room, ct);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Rooms.FindAsync([id], ct);
    }
}
