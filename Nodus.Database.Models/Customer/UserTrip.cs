using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class UserTrip
    {
        [Column("UserID")]
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public UserDetails User { get; set; }

        [Column("TripID")]
        [Required]
        public int TripId { get; set; }

        [ForeignKey("TripId")]
        public Trip Trip { get; set; }

        public bool CanUploadBills { get; set; }

        public double Budget { get; set; }
    }
}
