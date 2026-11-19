using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId);

public sealed record CancelBookingResult;

public sealed class CancelBookingHandler(
    IBookingRepository bookings,
    IUnitOfWork uow,
    TimeProvider timeProvider)
    : ICommandHandler<CancelBookingCommand, CancelBookingResult>
{
    public async Task<Result<CancelBookingResult>> HandleAsync(
        CancelBookingCommand command, CancellationToken ct = default)
    {
        var booking = await bookings.GetByIdAsync(command.BookingId, ct);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        // R8 & R11: Cancel with TimeProvider check (at least 60 minutes from now)
        var result = booking.Cancel(timeProvider);
        if (result.IsFailure)
            return result.Error;

        await bookings.UpdateAsync(booking, ct);
        await uow.SaveChangesAsync(ct);

        return new CancelBookingResult();
    }
}
