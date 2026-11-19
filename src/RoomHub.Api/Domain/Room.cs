namespace RoomHub.Api.Domain;

public sealed class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    private Room() { }

    public static Result<Room> Create(string? name, int capacity)
    {
        // R1: A room has a non-empty name and a strictly positive capacity.
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("InvalidName", "Room name is required.");

        if (capacity <= 0)
            return Error.Validation("InvalidCapacity", "Capacity must be positive.");

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };
    }

    public bool CanAccommodate(int attendees) => attendees <= Capacity;

    // R10: Rooms with capacity > 10 can be booked for at most 2 hours; <= 10 keep 4 hours.
    public TimeSpan GetMaxBookingDuration() =>
        Capacity > 10 ? TimeSpan.FromHours(2) : TimeSpan.FromHours(4);
}
