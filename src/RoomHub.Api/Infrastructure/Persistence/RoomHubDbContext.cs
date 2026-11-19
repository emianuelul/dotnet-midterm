using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class RoomHubDbContext(DbContextOptions<RoomHubDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired();
            entity.Property(r => r.Capacity).IsRequired();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.RoomId).IsRequired();
            entity.Property(b => b.BookedBy).IsRequired();
            entity.Property(b => b.StartUtc).IsRequired();
            entity.Property(b => b.EndUtc).IsRequired();
            entity.Property(b => b.Attendees).IsRequired();
            entity.Property(b => b.Status)
                .HasConversion<string>()
                .IsRequired();
        });
    }

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        await base.SaveChangesAsync(ct);
    }
}
