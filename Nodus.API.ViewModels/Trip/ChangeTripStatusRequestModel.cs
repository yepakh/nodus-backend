using Nodus.Database.Models.Customer;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Trip
{
    public class ChangeTripStatusRequestModel
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        public TripStatusEnum NewStatus { get; set; }
    }
}
