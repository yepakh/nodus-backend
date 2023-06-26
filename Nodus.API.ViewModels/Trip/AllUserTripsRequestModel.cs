using Nodus.API.Models.Common;
using Nodus.Database.Models.Customer;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Trip
{
    public class AllUserTripsRequestModel
    {
        [Required]
        public TripRole TripRole { get; set; }

        [Required]
        public TripStatusEnum TripStatus { get; set; }

        [Required]
        public RequestPagination Pagination { get; set; }
    }
}
