using FlightManagement.Models;
using FlightManagement.Repositories;

namespace FlightManagement.Services
{
    public class FlightService(
        IFlightRepository flightRepository,
        IAirportRepository airportRepository,
        IFlightCalculationService calculationService,
        ILogger<FlightService> logger) : IFlightService
    {
        private readonly IFlightRepository _flightRepository = flightRepository;
        private readonly IAirportRepository _airportRepository = airportRepository;
        private readonly IFlightCalculationService _calculationService = calculationService;
        private readonly ILogger<FlightService> _logger = logger;

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            return await _flightRepository.GetAllAsync();
        }

        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
            {
                _logger.LogWarning("Flight with ID {FlightId} not found", id);
                throw new KeyNotFoundException($"Flight with ID {id} not found");
            }
            return flight;
        }

        public async Task<Flight> CreateFlightAsync(string flightNumber, int departureAirportId, int destinationAirportId,
            DateTime departureDate, decimal fuelConsumptionPerKm, decimal takeoffFuel)
        {
            // Business validation
            if (departureAirportId == destinationAirportId)
            {
                throw new InvalidOperationException("Departure and destination airports must be different");
            }

            // Get airports
            var departureAirport = await _airportRepository.GetByIdAsync(departureAirportId);
            var destinationAirport = await _airportRepository.GetByIdAsync(destinationAirportId);

            if (departureAirport == null || destinationAirport == null)
            {
                throw new InvalidOperationException("Invalid airport selection");
            }

            // Create flight entity
            var flight = new Flight
            {
                FlightNumber = flightNumber,
                DepartureAirportId = departureAirportId,
                DestinationAirportId = destinationAirportId,
                DepartureDate = departureDate,
                FuelConsumptionPerKm = fuelConsumptionPerKm,
                TakeoffFuel = takeoffFuel,
                CreatedAt = DateTime.UtcNow
            };

            // Calculate metrics
            CalculateFlightMetrics(flight, departureAirport, destinationAirport);

            // Persist
            var createdFlight = await _flightRepository.AddAsync(flight);

            _logger.LogInformation("Flight {FlightNumber} created with ID {FlightId}", flightNumber, createdFlight.Id);

            return createdFlight;
        }

        public async Task<Flight> UpdateFlightAsync(int id, string flightNumber, int departureAirportId, int destinationAirportId,
            DateTime departureDate, decimal fuelConsumptionPerKm, decimal takeoffFuel)
        {
            var flight = await GetFlightByIdAsync(id);

            if (departureAirportId == destinationAirportId)
            {
                throw new InvalidOperationException("Departure and destination airports must be different");
            }

            var departureAirport = await _airportRepository.GetByIdAsync(departureAirportId);
            var destinationAirport = await _airportRepository.GetByIdAsync(destinationAirportId);

            if (departureAirport == null || destinationAirport == null)
            {
                throw new InvalidOperationException("Invalid airport selection");
            }

            flight.FlightNumber = flightNumber;
            flight.DepartureAirportId = departureAirportId;
            flight.DestinationAirportId = destinationAirportId;
            flight.DepartureDate = departureDate;
            flight.FuelConsumptionPerKm = fuelConsumptionPerKm;
            flight.TakeoffFuel = takeoffFuel;
            flight.ModifiedAt = DateTime.UtcNow;

            // Recalculate metrics
            CalculateFlightMetrics(flight, departureAirport, destinationAirport);

            await _flightRepository.UpdateAsync(flight);

            _logger.LogInformation("Flight {FlightNumber} (ID: {FlightId}) updated", flightNumber, id);

            return flight;
        }

        public async Task DeleteFlightAsync(int id)
        {
            var flight = await GetFlightByIdAsync(id);
            await _flightRepository.DeleteAsync(id);

            _logger.LogInformation("Flight {FlightNumber} (ID: {FlightId}) deleted", flight.FlightNumber, id);
        }

        public async Task<bool> ValidateAirportsAsync(int departureAirportId, int destinationAirportId)
        {
            if (departureAirportId == destinationAirportId)
            {
                return false;
            }

            var departureAirport = await _airportRepository.GetByIdAsync(departureAirportId);
            var destinationAirport = await _airportRepository.GetByIdAsync(destinationAirportId);

            return departureAirport != null && destinationAirport != null;
        }

        private void CalculateFlightMetrics(Flight flight, Airport departureAirport, Airport destinationAirport)
        {
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
