namespace Nodus.API.Models.TotalStatistics
{
    public class TotalStatisticsFilterRequestModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> TripIds { get; set; }
        public List<Guid> UserIds { get; set; }
        public List<int> CategoryIds { get; set; }

        public TotalStatisticsFilterRequestModel()
        {
            TripIds = new List<int>();
            UserIds = new List<Guid>();
            CategoryIds = new List<int>();
        }
    }
}
