using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingRequest(
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);

public static class CreateBookingEndpoint
{
    public static RouteGroupBuilder MapCreateBooking(this RouteGroupBuilder group, string endpointName = "CreateBooking")
    {
        group.MapPost("/{roomId:guid}/bookings", async (
            Guid roomId,
            CreateBookingRequest request,
            ICommandHandler<CreateBookingCommand, CreateBookingResult> handler,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new CreateBookingCommand(
                    roomId,
                    request.BookedBy,
                    request.StartUtc,
                    request.EndUtc,
                    request.Attendees),
                ct);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var booking = result.Value;
            var requestPath = httpContext.Request.Path.Value ?? "";
            string location;
            if (requestPath.StartsWith("/api/v1"))
                location = $"/api/v1/bookings/{booking.Id}";
            else if (requestPath.StartsWith("/api/v2"))
                location = $"/api/v2/bookings/{booking.Id}";
            else
                location = $"/api/bookings/{booking.Id}";

            return Results.Created(
                location,
                new { booking.Id, booking.RoomId, booking.Status });
        })
        .WithName(endpointName)
        .WithSummary("Create a new booking for a room");

        return group;
    }
}
