namespace FlightManagement.DTO
{
    public class FlightUpdateDTO
    {
        public int Id { get; set; }
        public required string FlightNumber { get; set; }
        public int DepartureAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal FuelConsumptionPerKm { get; set; }
        public decimal TakeoffFuel { get; set; }
    }
}
