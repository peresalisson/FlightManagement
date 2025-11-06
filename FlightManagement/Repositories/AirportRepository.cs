using FlightManagement.Data;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Repositories
{
    public class AirportRepository(ApplicationDbContext context) : IAirportRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Airport>> GetAllAsync()
        {
            return await _context.Airports
                .OrderBy(a => a.IataCode)
                .ToListAsync();
        }

        public async Task<Airport> GetByIdAsync(int id)
        {
            return await _context.Airports
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Airport> GetByIataCodeAsync(string iataCode)
        {
            return await _context.Airports
                .FirstOrDefaultAsync(a => a.IataCode == iataCode);
        }
    }
}
