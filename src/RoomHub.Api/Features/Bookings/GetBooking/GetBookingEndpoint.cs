using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public static RouteGroupBuilder MapGetBooking(this RouteGroupBuilder group, string endpointName = "GetBookingById")
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookingQuery, GetBookingResult> handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetBookingQuery(id), ct);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var booking = result.Value;
            return Results.Ok(new
            {
                booking.Id,
                booking.RoomId,
                booking.BookedBy,
                booking.StartUtc,
                booking.EndUtc,
                booking.Attendees,
                booking.Status
            });
        })
        .WithName(endpointName)
        .WithSummary("Get a booking by ID");

        return group;
    }
}
