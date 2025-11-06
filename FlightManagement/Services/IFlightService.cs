using FlightManagement.DTO;
using FlightManagement.Models;

namespace FlightManagement.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> GetAllFlightsAsync(CancellationToken cancellationToken);
        Task<Flight> GetFlightByIdAsync(int id);
        Task<Flight> CreateFlightAsync(CreateFlightDTO create);
        Task<Flight> UpdateFlightAsync(FlightUpdateDTO update);
        Task DeleteFlightAsync(int id);
        Task<bool> ValidateAirportsAsync(int departureAirportId, int destinationAirportId);
    }
}
