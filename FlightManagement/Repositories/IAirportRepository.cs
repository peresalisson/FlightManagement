using Microsoft.EntityFrameworkCore;
using FlightManagement.Data;
using FlightManagement.Models;

namespace FlightManagement.Repositories
{
    public interface IAirportRepository
    {
        Task<IEnumerable<Airport>> GetAllAsync();
        Task<Airport> GetByIdAsync(int id);
        Task<Airport> GetByIataCodeAsync(string iataCode);
    }
}
