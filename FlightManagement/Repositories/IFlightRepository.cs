using FlightManagement.Models;

namespace FlightManagement.Repositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Flight> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default);
        Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}
