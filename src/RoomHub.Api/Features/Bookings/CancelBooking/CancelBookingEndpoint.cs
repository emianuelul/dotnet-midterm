using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static RouteGroupBuilder MapCancelBooking(this RouteGroupBuilder group, string endpointName = "CancelBooking")
    {
        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelBookingCommand, CancelBookingResult> handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new CancelBookingCommand(id), ct);

            if (result.IsFailure)
                return result.Error.ToProblem();

            return Results.NoContent();
        })
        .WithName(endpointName)
        .WithSummary("Cancel a booking by ID");

        return group;
    }

    public static RouteGroupBuilder MapCancelBookingDelete(this RouteGroupBuilder group, string endpointName = "CancelBookingDelete")
    {
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<CancelBookingCommand, CancelBookingResult> handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new CancelBookingCommand(id), ct);

            if (result.IsFailure)
                return result.Error.ToProblem();

            return Results.NoContent();
        })
        .WithName(endpointName)
        .WithSummary("Cancel/delete a booking by ID (RESTful verb)");

        return group;
    }
}
