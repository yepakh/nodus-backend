using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class Bill : DbEntityBase
    {
        public Bill()
        {
            Documents = new HashSet<Document>();
            HistoricalBills = new HashSet<HistoricalBill>();
        }

        [Required]
        public bool IsActive { get; set; }

        public int TripId { get; set; }

        public Trip Trip { get; set; }

        [Required]
        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        public string Name { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Desciption { get; set; }

        public double Summary { get; set; }

        public DateTime DateTimeCreated { get; set; }

        public DateTime DateTimeEdited { get; set; }

        [Column("CreatorID")]
        [Required]
        public Guid CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public UserDetails Creator { get; set; }

        [Column("EditorID")]
        [Required]
        public Guid EditorId { get; set; }

        [ForeignKey("EditorId")]
        public UserDetails Editor { get; set; }

        [Column("StatusID")]
        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public BillStatus Status { get; set; }

        [Column("BillCategoryID")]
        [Required]
        public int BillCategoryId { get; set; }

        [ForeignKey("BillCategoryId")]
        public BillCategory BillCategory { get; set; }

        public ICollection<Document> Documents { get; private set; }
        public ICollection<HistoricalBill> HistoricalBills { get; private set; }
    }
}