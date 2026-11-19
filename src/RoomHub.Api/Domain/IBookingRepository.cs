namespace RoomHub.Api.Domain;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken ct = default);
    Task<int> CountActiveForParticipantOnDateAsync(string bookedBy, DateTime dateUtc, CancellationToken ct = default);
    Task UpdateAsync(Booking booking, CancellationToken ct = default);
}
