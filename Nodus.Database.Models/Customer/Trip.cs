using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class Trip : DbEntityBase
    {
        public Trip()
        {
            UserTrips = new HashSet<UserTrip>();
            Bills = new HashSet<Bill>();
        }

        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Description { get; set; }

        public double Budget { get; set; }

        [Column("CreatorID")]
        [Required]
        public Guid CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public UserDetails Creator { get; set; }

        [Required]
        public DateTime DateTimeCreated { get; set; }

        public DateTime StartOfTrip { get; set; }

        public DateTime EndOfTrip { get; set; }

        public ICollection<UserTrip> UserTrips { get; private set; }

        public ICollection<Bill> Bills { get; private set; }

        public TripStatusEnum Status { get; set; }
    }
}
