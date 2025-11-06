using Microsoft.EntityFrameworkCore;
using FlightManagement.Models;

namespace FlightManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Airport> Airports { get; set; }
        public DbSet<Flight> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Airport>(entity =>
            {
                entity.HasIndex(e => e.IataCode).IsUnique();
                entity.Property(e => e.IataCode).IsRequired();
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasOne(f => f.DepartureAirport)
                    .WithMany(a => a.DepartureFlights)
                    .HasForeignKey(f => f.DepartureAirportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.DestinationAirport)
                    .WithMany(a => a.DestinationFlights)
                    .HasForeignKey(f => f.DestinationAirportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(f => f.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Airports.Any())
            {
                return;
            }

            var airports = new Airport[]
            {
                new()
                {
                    IataCode = "JFK",
                    Name = "John F. Kennedy International Airport",
                    City = "New York",
                    Country = "USA",
                    Latitude = 40.6413,
                    Longitude = -73.7781,
                },
                new()
                {
                    IataCode = "LAX",
                    Name = "Los Angeles International Airport",
                    City = "Los Angeles",
                    Country = "USA",
                    Latitude = 33.9416,
                    Longitude = -118.4085
                },
                new()
                {
                    IataCode = "LHR",
                    Name = "London Heathrow Airport",
                    City = "London",
                    Country = "UK",
                    Latitude = 51.4700,
                    Longitude = -0.4543
                },
                new()
                {
                    IataCode = "CDG",
                    Name = "Charles de Gaulle Airport",
                    City = "Paris",
                    Country = "France",
                    Latitude = 49.0097,
                    Longitude = 2.5479
                },
                new() {
                    IataCode = "DXB",
                    Name = "Dubai International Airport",
                    City = "Dubai",
                    Country = "UAE",
                    Latitude = 25.2532,
                    Longitude = 55.3657
                },
                new() {
                    IataCode = "NRT",
                    Name = "Narita International Airport",
                    City = "Tokyo",
                    Country = "Japan",
                    Latitude = 35.7720,
                    Longitude = 140.3929
                }
            };

            context.Airports.AddRange(airports);
            context.SaveChanges();
        }
    }
}