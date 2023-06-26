using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class Document
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Link { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime DateTimeCreated { get; set; }

        [Required]
        [Column("CreatorID")]
        public Guid CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public UserDetails Creator { get; set; }

        [Column("DeactivatorID")]
        public Guid? DeactivatorId { get; set; }

        [ForeignKey("DeactivatorId")]
        public UserDetails Deactivator { get; set; }

        [Required]
        [Column("BillID")]
        public int BillId { get; set; }

        [ForeignKey("BillId")]
        public Bill Bill { get; set; }

        public ICollection<HistoricalBill> HistoricalBills { get; set; }
    }
}