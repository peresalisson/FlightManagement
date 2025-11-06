using System.Threading;
using FlightManagement.Data;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Repositories
{
    public class FlightRepository(ApplicationDbContext context) : IFlightRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Flight>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Flights
                .AsNoTracking()
                .Include(f => f.DepartureAirport)
                .Include(f => f.DestinationAirport)
                .OrderByDescending(f => f.DepartureDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Flight> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Flights
                .AsNoTracking()
                .Include(f => f.DepartureAirport)
                .Include(f => f.DestinationAirport)
                .SingleOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default)
        {
            flight.CreatedAt = DateTime.UtcNow;
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync(cancellationToken);
            return flight;
        }

        public async Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default)
        {
            flight.ModifiedAt = DateTime.UtcNow;
            _context.Entry(flight).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var flight = await _context.Flights
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Flights.AnyAsync(f => f.Id == id, cancellationToken);
        }
    }
}
