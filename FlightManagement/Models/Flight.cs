using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightManagement.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Flight Number")]
        public required string FlightNumber { get; set; }

        [Required]
        [Display(Name = "Departure Airport")]
        public int DepartureAirportId { get; set; }

        [ForeignKey("DepartureAirportId")]
        public virtual Airport? DepartureAirport { get; set; }

        [Required]
        [Display(Name = "Destination Airport")]
        public int DestinationAirportId { get; set; }

        [ForeignKey("DestinationAirportId")]
        public virtual Airport? DestinationAirport { get; set; }

        [Required]
        [Display(Name = "Departure Date")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureDate { get; set; }

        [Required]
        [Range(0.1, 1000)]
        [Display(Name = "Fuel Consumption (L/km)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FuelConsumptionPerKm { get; set; }

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Takeoff Fuel (L)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TakeoffFuel { get; set; }

        [Display(Name = "Distance (km)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CalculatedDistance { get; set; }

        [Display(Name = "Required Fuel (L)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RequiredFuel { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }
    }
}
