namespace Nodus.API.Models.Bill
{
    public class UpdateBillRequestModel
    {
        public int BillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Sumary { get; set; }
        public int TripId { get; set; }
        public int CategoryId { get; set; }
        public List<string> DocumentsUrls { get; set; }
    }
}
