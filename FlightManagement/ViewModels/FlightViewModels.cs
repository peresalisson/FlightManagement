using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightManagement.ViewModels
{
    public class FlightCreateViewModel
    {
        [Required]
        [StringLength(10)]
        [Display(Name = "Flight Number")]
        public string? FlightNumber { get; set; }

        [Required]
        [Display(Name = "Departure Airport")]
        public int DepartureAirportId { get; set; }

        [Required]
        [Display(Name = "Destination Airport")]
        public int DestinationAirportId { get; set; }

        [Required]
        [Display(Name = "Departure Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureDate { get; set; }

        [Required]
        [Range(0.1, 1000)]
        [Display(Name = "Fuel Consumption per Km (L/km)")]
        public decimal FuelConsumptionPerKm { get; set; }

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Takeoff Fuel (Liters)")]
        public decimal TakeoffFuel { get; set; }

        public SelectList? DepartureAirports { get; set; }
        public SelectList? DestinationAirports { get; set; }
    }

    public class FlightEditViewModel : FlightCreateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Calculated Distance (km)")]
        public decimal? CalculatedDistance { get; set; }

        [Display(Name = "Required Fuel (L)")]
        public decimal? RequiredFuel { get; set; }
    }

    public class FlightReportViewModel
    {
        public List<FlightReportItem>? Flights { get; set; }
        public FlightReportSummary? Summary { get; set; }
    }

    public class FlightReportItem
    {
        public int Id { get; set; }
        public string? FlightNumber { get; set; }
        public string? DepartureAirport { get; set; }
        public string? DestinationAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal? CalculatedDistance { get; set; }
        public decimal FuelConsumptionPerKm { get; set; }
        public decimal TakeoffFuel { get; set; }
        public decimal? RequiredFuel { get; set; }
    }

    public class FlightReportSummary
    {
        public int TotalFlights { get; set; }
        public decimal TotalDistance { get; set; }
        public decimal TotalFuelRequired { get; set; }
        public decimal AverageDistance { get; set; }
        public decimal AverageFuelPerFlight { get; set; }
    }
}