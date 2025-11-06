using FlightManagement.Models;

namespace FlightManagement.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task<Flight> GetFlightByIdAsync(int id);
        Task<Flight> CreateFlightAsync(string flightNumber, int departureAirportId, int destinationAirportId,
            DateTime departureDate, decimal fuelConsumptionPerKm, decimal takeoffFuel);
        Task<Flight> UpdateFlightAsync(int id, string flightNumber, int departureAirportId, int destinationAirportId,
            DateTime departureDate, decimal fuelConsumptionPerKm, decimal takeoffFuel);
        Task DeleteFlightAsync(int id);
        Task<bool> ValidateAirportsAsync(int departureAirportId, int destinationAirportId);
    }
}
