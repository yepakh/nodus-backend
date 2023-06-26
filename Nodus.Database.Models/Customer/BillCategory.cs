namespace Nodus.Database.Models.Customer
{
    public class BillCategory : DbEntityBase
    {
        public BillCategory()
        {
            Bills = new HashSet<Bill>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Bill> Bills { get; private set; }

        public bool IsIncludedInDailyAllowance { get; set; }
    }
}
