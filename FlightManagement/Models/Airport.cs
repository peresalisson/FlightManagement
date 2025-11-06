using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightManagement.Models
{
    public class Airport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [Display(Name = "IATA Code")]
        public required string IataCode { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Airport Name")]
        public required string Name { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [StringLength(100)]
        public required string City { get; set; }

        [StringLength(100)]
        public required string Country { get; set; }

        public virtual ICollection<Flight>? DepartureFlights { get; set; }
        public virtual ICollection<Flight>? DestinationFlights { get; set; }
    }
}