using FlightManagement.Data;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Repositories
{
    public class AirportRepository(ApplicationDbContext context) : IAirportRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Airport>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Airports
                .OrderBy(a => a.IataCode)
                .ToListAsync();
        }

        public async Task<Airport> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Airports
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Airport> GetByIataCodeAsync(string iataCode, CancellationToken cancellationToken)
        {
            return await _context.Airports
                .FirstOrDefaultAsync(a => a.IataCode == iataCode);
        }
    }
}
