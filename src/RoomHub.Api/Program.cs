using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// TimeProvider
builder.Services.AddSingleton(TimeProvider.System);

// EF Core SQLite + Unit of Work + Repositories
builder.Services.AddDbContext<RoomHubDbContext>(options =>
{
    options.UseSqlite("Data Source=roomhub.db");
});
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// CQRS Handlers
builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, CreateRoomResult>, CreateRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, GetRoomResult>, GetRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, CreateBookingResult>, CreateBookingHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, GetBookingResult>, GetBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand, CancelBookingResult>, CancelBookingHandler>();

var app = builder.Build();

// Ensure SQLite database and tables are created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "RoomHub API is running.");

// Original API contract endpoints (for automated test suite)
var api = app.MapGroup("/api");
MapEndpoints(api, "Default");

// Version 1 endpoints (prefixed with /api/v1)
var v1 = api.MapGroup("/v1");
MapEndpoints(v1, "V1");

// Version 2 endpoints (prefixed with /api/v2, RESTful conventions, enhanced features)
var v2 = api.MapGroup("/v2");
MapV2Endpoints(v2);

app.Run();

static void MapEndpoints(RouteGroupBuilder root, string versionSuffix)
{
    var rooms = root.MapGroup("/rooms").WithTags($"Rooms {versionSuffix}");
    rooms.MapCreateRoom($"CreateRoom_{versionSuffix}");
    rooms.MapGetRoom($"GetRoom_{versionSuffix}");
    rooms.MapCreateBooking($"CreateBooking_{versionSuffix}");

    var bookings = root.MapGroup("/bookings").WithTags($"Bookings {versionSuffix}");
    bookings.MapGetBooking($"GetBooking_{versionSuffix}");
    bookings.MapCancelBooking($"CancelBooking_{versionSuffix}");
}

static void MapV2Endpoints(RouteGroupBuilder v2)
{
    var rooms = v2.MapGroup("/rooms").WithTags("Rooms V2");
    rooms.MapCreateRoom("CreateRoom_V2");
    rooms.MapGetRoom("GetRoom_V2");
    rooms.MapCreateBooking("CreateBooking_V2");

    // v2 enhanced: list active bookings for a room (RESTful sub-resource)
    rooms.MapGet("/{roomId:guid}/bookings", async (
        Guid roomId,
        IRoomRepository roomsRepo,
        IBookingRepository bookingsRepo,
        CancellationToken ct) =>
    {
        var room = await roomsRepo.GetByIdAsync(roomId, ct);
        if (room is null)
            return Results.Problem(detail: "The room does not exist.", statusCode: 404, title: "RoomNotFound");

        var activeBookings = await bookingsRepo.GetActiveForRoomAsync(roomId, ct);
        var response = activeBookings.Select(b => new
        {
            b.Id,
            b.RoomId,
            b.BookedBy,
            b.StartUtc,
            b.EndUtc,
            b.Attendees,
            Status = b.Status.ToString()
        });
        return Results.Ok(response);
    })
    .WithName("GetRoomBookings_V2")
    .WithSummary("Get active bookings for a room");

    var bookings = v2.MapGroup("/bookings").WithTags("Bookings V2");
    bookings.MapGetBooking("GetBooking_V2");
    bookings.MapCancelBooking("CancelBooking_V2");
    bookings.MapCancelBookingDelete("CancelBooking_V2_Delete");
}
