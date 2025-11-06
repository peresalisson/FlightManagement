using FlightManagement.Data;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Repositories
{
    public class FlightRepository(ApplicationDbContext context) : IFlightRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.DestinationAirport)
                .OrderByDescending(f => f.DepartureDate)
                .ToListAsync();
        }

        public async Task<Flight> GetByIdAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.DestinationAirport)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Flight> AddAsync(Flight flight)
        {
            flight.CreatedAt = DateTime.UtcNow;
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();
            return flight;
        }

        public async Task UpdateAsync(Flight flight)
        {
            flight.ModifiedAt = DateTime.UtcNow;
            _context.Entry(flight).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Flights.AnyAsync(f => f.Id == id);
        }
    }
}
