using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Customer
{
    public class BillStatus : DbEntityBase
    {
        [Required]
        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        public string Name { get; set; }

        public BillStatus()
        {
            Bills = new HashSet<Bill>();
            HistoricalBills = new HashSet<HistoricalBill>();
        }
        public ICollection<Bill> Bills { get; private set; }
        public ICollection<HistoricalBill> HistoricalBills { get; private set; }
    }
}