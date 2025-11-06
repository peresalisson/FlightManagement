using FlightManagement.DTO;
using FlightManagement.Models;
using FlightManagement.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _flightRepository.GetAllAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id).ConfigureAwait(false);

            if (flight == null)
            {
                _logger.LogWarning("Flight with ID {FlightId} not found", id);
                throw new KeyNotFoundException($"Flight with ID {id} not found");
            }

            return flight;
        }

        public async Task<Flight> CreateFlightAsync(CreateFlightDTO create)
        {
            ArgumentNullException.ThrowIfNull(create);

            if (create.DepartureAirportId == create.DestinationAirportId)
            {
                throw new InvalidOperationException("Departure and destination airports must be different");
            }

            var departureAirport = await _airportRepository.GetByIdAsync(create.DepartureAirportId, CancellationToken.None);
            var destinationAirport = await _airportRepository.GetByIdAsync(create.DestinationAirportId, CancellationToken.None);

            if (departureAirport == null || destinationAirport == null)
            {
                throw new InvalidOperationException("Invalid airport selection");
            }

            var flight = new Flight
            {
                FlightNumber = create.FlightNumber,
                DepartureAirportId = create.DepartureAirportId,
                DestinationAirportId = create.DestinationAirportId,
                DepartureDate = create.DepartureDate,
                FuelConsumptionPerKm = create.FuelConsumptionPerKm,
                TakeoffFuel = create.TakeoffFuel,
                CreatedAt = DateTime.UtcNow
            };

            CalculateFlightMetrics(flight, departureAirport, destinationAirport);

            var createdFlight = await _flightRepository.AddAsync(flight).ConfigureAwait(false);

            _logger.LogInformation("Flight {FlightNumber} created with ID {FlightId}", create.FlightNumber, createdFlight.Id);

            return createdFlight;
        }

        public async Task<Flight> UpdateFlightAsync(FlightUpdateDTO update)
        {
            var existing = await _flightRepository.GetByIdAsync(update.Id) ?? throw new KeyNotFoundException();
            existing.FlightNumber = update.FlightNumber;
            existing.DepartureAirportId = update.DepartureAirportId;
            existing.DestinationAirportId = update.DestinationAirportId;
            existing.DepartureDate = update.DepartureDate;
            existing.FuelConsumptionPerKm = update.FuelConsumptionPerKm;
            existing.TakeoffFuel = update.TakeoffFuel;
            existing.ModifiedAt = DateTime.UtcNow;

            await _flightRepository.UpdateAsync(existing).ConfigureAwait(false);
            return existing;
        }

        public async Task DeleteFlightAsync(int id)
        {
            var flight = await GetFlightByIdAsync(id);
            await _flightRepository.DeleteAsync(id).ConfigureAwait(false);

            _logger.LogInformation("Flight {FlightNumber} (ID: {FlightId}) deleted", flight.FlightNumber, id);
        }

        public async Task<bool> ValidateAirportsAsync(int departureAirportId, int destinationAirportId)
        {
            if (departureAirportId == destinationAirportId)
            {
                return false;
            }

            var departureAirport = await _airportRepository.GetByIdAsync(departureAirportId, CancellationToken.None).ConfigureAwait(false);
            var destinationAirport = await _airportRepository.GetByIdAsync(destinationAirportId, CancellationToken.None).ConfigureAwait(false);

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
