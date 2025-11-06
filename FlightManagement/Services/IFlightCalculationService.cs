using FlightManagement.Models;

namespace FlightManagement.Services
{
    public interface IFlightCalculationService
    {
        double CalculateDistance(Airport departure, Airport destination);
        decimal CalculateFuelRequired(double distanceKm, decimal fuelConsumptionPerKm, decimal takeoffFuel);
    }
}