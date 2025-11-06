using FlightManagement.Models;

namespace FlightManagement.Services
{
    public class FlightCalculationService : IFlightCalculationService
    {
        private const double EarthRadiusKm = 6371.0;
        
        /// <summary>
        /// Calculates the great-circle distance between two GPS coordinates using the Haversine formula
        /// </summary>
        public double CalculateDistance(Airport departure, Airport destination)
        {
            if (departure == null || destination == null)
                throw new ArgumentNullException("Airports cannot be null");
        
            var lat1Rad = ToRadians(departure.Latitude);
            var lat2Rad = ToRadians(departure.Latitude);
            var deltaLatRad = ToRadians(destination.Latitude - departure.Latitude);
            var deltaLonRad = ToRadians(destination.Longitude - departure.Longitude);

            var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        public decimal CalculateFuelRequired(double distanceKm, decimal fuelConsumptionPerKm, decimal takeoffFuel)
        {
            if (distanceKm < 0 || fuelConsumptionPerKm < 0 || takeoffFuel < 0)
                throw new ArgumentException("Values cannot be negative");

            var cruiseFuel = (decimal)distanceKm * fuelConsumptionPerKm;
            return cruiseFuel + takeoffFuel;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
