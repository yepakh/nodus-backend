using Nodus.API.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Trip
{
    public class GetTripUsersRequestModel
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        public RequestPagination Pagination { get; set; }
    }
}
