namespace RoomHub.Api.Domain;

public sealed class Booking
{
    public static readonly TimeSpan StandardMaxDuration = TimeSpan.FromHours(4);

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    private Booking() { }

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        TimeProvider timeProvider,
        IReadOnlyList<Booking> existingActiveRoomBookings,
        int participantActiveBookingsTodayCount = 0)
    {
        // R2: bookedBy is required.
        if (string.IsNullOrWhiteSpace(bookedBy))
            return Error.Validation("InvalidBookedBy", "BookedBy is required.");

        // R3: endUtc must be strictly after startUtc.
        if (endUtc <= startUtc)
            return Error.Validation("InvalidRange", "The end must be after the start.");

        // R4: attendees is strictly positive and at most the room capacity.
        if (attendees <= 0)
            return Error.Validation("InvalidAttendees", "Attendees must be positive.");

        if (!room.CanAccommodate(attendees))
            return Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats.");

        // R5: A booking cannot start in the past. "Now" comes from TimeProvider.
        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (startUtc < now)
            return Error.Validation("StartsInPast", "A booking cannot start in the past.");

        // R6 & R10: Duration limits (capacity > 10 => max 2h; <= 10 => max 4h)
        var maxDuration = room.GetMaxBookingDuration();
        if ((endUtc - startUtc) > maxDuration)
        {
            var maxHours = (int)maxDuration.TotalHours;
            return Error.Validation("TooLong", $"The maximum duration of a booking for this room is {maxHours} hours.");
        }

        // R7: Two active bookings of the same room cannot overlap (409).
        // Bookings that only touch (one ends exactly when the other starts) do not overlap.
        foreach (var existing in existingActiveRoomBookings)
        {
            if (existing.OverlapsWith(startUtc, endUtc))
                return Error.Conflict("RoomOccupied", "The room is occupied in this interval.");
        }

        // R9: A participant cannot have more than 2 active bookings per day across all rooms together (409).
        if (participantActiveBookingsTodayCount >= 2)
            return Error.Conflict("DailyBookingLimitExceeded", "A participant cannot have more than 2 active bookings per day.");

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy.Trim(),
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    public bool OverlapsWith(DateTime startUtc, DateTime endUtc)
    {
        return Status == BookingStatus.Active && StartUtc < endUtc && startUtc < EndUtc;
    }

    // R8: Cancelling moves a booking from Active to Cancelled.
    // Cancelling an already cancelled booking gives 409.
    // R11: A booking can be cancelled only if it starts at least 60 minutes from now (TimeProvider). Otherwise 409.
    public Result<Booking> Cancel(TimeProvider timeProvider)
    {
        if (Status == BookingStatus.Cancelled)
            return Error.Conflict("AlreadyCancelled", "The booking is already cancelled.");

        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (StartUtc - now < TimeSpan.FromMinutes(60))
            return Error.Conflict("CancellationWindowClosed", "A booking can be cancelled only if it starts at least 60 minutes from now.");

        Status = BookingStatus.Cancelled;
        return this;
    }
}
