using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.SingleTripStatistics
{
    public class GetTripStatisticsRequestModel
    {
        [FromQuery]
        public int TripId { get; set; }

        [FromQuery]
        public List<Guid> UserIds { get; set; }

        [FromQuery]
        public List<int> CategoryIds { get; set; }

        public GetTripStatisticsRequestModel()
        {
            UserIds = new List<Guid>();
            CategoryIds = new List<int>();
        }
    }
}
