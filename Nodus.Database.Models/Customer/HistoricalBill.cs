using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class HistoricalBill : DbEntityBase
    {
        public HistoricalBill()
        {
            Documents = new HashSet<Document>();
        }

        [Required]
        [Column("BillID")]
        public int BillId { get; set; }

        [ForeignKey("BillId")]
        public Bill Bill { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Desciption { get; set; }

        public double? Summary { get; set; }

        public DateTime DateTimeEdit { get; set; }

        [Required]
        [Column("EditorID")]
        public Guid EditorId { get; set; }

        [ForeignKey("EditorId")]
        public UserDetails Editor { get; set; }

        [Required]
        [Column("StatusID")]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public BillStatus Status { get; set; }

        public ICollection<Document> Documents { get; set; }
    }
}
