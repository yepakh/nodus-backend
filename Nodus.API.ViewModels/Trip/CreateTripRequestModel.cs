using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Trip
{
    public class CreateTripRequestModel
    {
        [Required]
        public List<TripUserRequestModel> Users { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Budget { get; set; }

        [Required]
        public DateTime DateTimeStart { get; set; }

        [Required]
        public DateTime DateTimeEnd { get; set; }
    }
}
