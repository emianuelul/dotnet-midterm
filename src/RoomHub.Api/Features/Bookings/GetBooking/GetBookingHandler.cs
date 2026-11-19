using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid Id);

public sealed record GetBookingResult(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public sealed class GetBookingHandler(IBookingRepository bookings)
    : IQueryHandler<GetBookingQuery, GetBookingResult>
{
    public async Task<Result<GetBookingResult>> HandleAsync(
        GetBookingQuery query, CancellationToken ct = default)
    {
        var booking = await bookings.GetByIdAsync(query.Id, ct);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        return new GetBookingResult(
            booking.Id,
            booking.RoomId,
            booking.BookedBy,
            booking.StartUtc,
            booking.EndUtc,
            booking.Attendees,
            booking.Status.ToString());
    }
}
