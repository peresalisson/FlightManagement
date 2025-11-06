using FlightManagement.Models;
using FlightManagement.Repositories;

namespace FlightManagement.Services
{
    public class FlightService(
        IFlightRepository flightRepository,
        IAirportRepository airportRepository,
        IFlightCalculationService calculationService) : IFlightService
    {
        private readonly IFlightRepository _flightRepository = flightRepository;
        private readonly IAirportRepository _airportRepository = airportRepository;
        private readonly IFlightCalculationService _calculationService = calculationService;

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            return await _flightRepository.GetAllAsync();
        }

        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            return await _flightRepository.GetByIdAsync(id);
        }

        public async Task<Flight> CreateFlightAsync(Flight flight)
        {
            await CalculateFlightMetrics(flight);
            return await _flightRepository.AddAsync(flight);
        }

        public async Task UpdateFlightAsync(Flight flight)
        {
            await CalculateFlightMetrics(flight);
            await _flightRepository.UpdateAsync(flight);
        }

        public async Task DeleteFlightAsync(int id)
        {
            await _flightRepository.DeleteAsync(id);
        }

        private async Task CalculateFlightMetrics(Flight flight)
        {
            var departureAirport = await _airportRepository.GetByIdAsync(flight.DepartureAirportId);
            var destinationAirport = await _airportRepository.GetByIdAsync(flight.DestinationAirportId);

            if (departureAirport == null || destinationAirport == null)
                throw new InvalidOperationException("Invalid airport selection");

            var distance = _calculationService.CalculateDistance(departureAirport, destinationAirport);
            flight.CalculatedDistance = Math.Round((decimal)distance, 2);

            flight.RequiredFuel = Math.Round(
                _calculationService.CalculateFuelRequired(
                    distance,
                    flight.FuelConsumptionPerKm,
                    flight.TakeoffFuel
                ), 2);
        }
    }
}
