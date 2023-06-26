namespace Nodus.API.Models.Trip
{
    public class TripUserRequestModel
    {
        public Guid UserId { get; set; }

        public bool CanUploadBills { get; set; }

        public double Budget { get; set; }
    }
}
