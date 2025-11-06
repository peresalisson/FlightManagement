using Microsoft.EntityFrameworkCore;
using FlightManagement.Data;
using FlightManagement.Models;

namespace FlightManagement.Repositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllAsync();
        Task<Flight> GetByIdAsync(int id);
        Task<Flight> AddAsync(Flight flight);
        Task UpdateAsync(Flight flight);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
