using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Trip
{
    public class AddUserToTripRequestModel
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        public List<TripUserRequestModel> Users { get; set; }
    }
}
