using Microsoft.AspNetCore.Mvc;
using Nodus.API.Models.Common;

namespace Nodus.API.Models.SingleTripStatistics
{
    public class GetTripStatisticsTotalBillsRequestModel
    {
        [FromQuery]
        public RequestPagination Pagination { get; set; }
        [FromQuery]
        public GetTripStatisticsRequestModel Filters { get; set; }
    }
}
