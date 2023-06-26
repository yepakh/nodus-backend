using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class UserDetails
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = EntityConstants.DEFAULT_EMAIL_TYPE)]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        public string FirstName { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        public string LastName { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_EMAIL_TYPE)]
        public string Address { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Notes { get; set; }

        public UserDetails()
        {
            CreatedTrips = new HashSet<Trip>();
            UserTrips = new HashSet<UserTrip>();
            CreatedBills = new HashSet<Bill>();
            EditedBills = new HashSet<Bill>();
            HistoricalBillsEdited = new HashSet<HistoricalBill>();
            CreatedDocuments = new HashSet<Document>();
            DocumentsDeactivated = new HashSet<Document>();
        }
        public ICollection<Trip> CreatedTrips { get; private set; }
        public ICollection<UserTrip> UserTrips { get; private set; }
        public ICollection<Bill> CreatedBills { get; private set; }
        public ICollection<Bill> EditedBills { get; private set; }
        public ICollection<HistoricalBill> HistoricalBillsEdited { get; private set; }
        public ICollection<Document> CreatedDocuments { get; private set; }
        public ICollection<Document> DocumentsDeactivated { get; private set; }
    }
}
