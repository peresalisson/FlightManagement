using Microsoft.EntityFrameworkCore;
using FlightManagement.Data;
using FlightManagement.Models;

namespace FlightManagement.Repositories
{
    public interface IAirportRepository
    {
        Task<IEnumerable<Airport>> GetAllAsync(CancellationToken cancellationToken);
        Task<Airport> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Airport> GetByIataCodeAsync(string iataCode, CancellationToken cancellationToken);
    }
}
