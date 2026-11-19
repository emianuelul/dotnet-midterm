using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);

public sealed record CreateBookingResult(Guid Id, Guid RoomId, string Status);

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork uow,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<Result<CreateBookingResult>> HandleAsync(
        CreateBookingCommand command, CancellationToken ct = default)
    {
        // ASSIGNMENT.md: When the room does not exist, 404 is returned before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, ct);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        var activeBookings = await bookings.GetActiveForRoomAsync(command.RoomId, ct);

        var participantTodayCount = 0;
        if (!string.IsNullOrWhiteSpace(command.BookedBy))
        {
            participantTodayCount = await bookings.CountActiveForParticipantOnDateAsync(
                command.BookedBy, command.StartUtc, ct);
        }

        var result = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            timeProvider,
            activeBookings,
            participantTodayCount);

        if (result.IsFailure)
            return result.Error;

        var booking = result.Value;
        await bookings.AddAsync(booking, ct);
        await uow.SaveChangesAsync(ct);

        return new CreateBookingResult(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
