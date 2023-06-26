using System;

namespace Nodus.API.Models.Bill
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Sumary { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeEdited { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string EditedById { get; set; }
        public string EditedByName { get; set; }
        public int TripId { get; set; }
        public int Status { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> DocumentsIds { get; set; }
        public List<string> DocumentsUrls { get; set; }
    }
}
