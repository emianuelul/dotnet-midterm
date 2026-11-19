using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        await db.Bookings.AddAsync(booking, ct);
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Bookings.FindAsync([id], ct);
    }

    public async Task<List<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        return await db.Bookings
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
            .ToListAsync(ct);
    }

    public async Task<int> CountActiveForParticipantOnDateAsync(
        string bookedBy, DateTime dateUtc, CancellationToken ct = default)
    {
        var trimmed = bookedBy.Trim();
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        return await db.Bookings
            .Where(b => b.BookedBy == trimmed
                     && b.Status == BookingStatus.Active
                     && b.StartUtc >= dayStart
                     && b.StartUtc < dayEnd)
            .CountAsync(ct);
    }

    public Task UpdateAsync(Booking booking, CancellationToken ct = default)
    {
        db.Bookings.Update(booking);
        return Task.CompletedTask;
    }
}
