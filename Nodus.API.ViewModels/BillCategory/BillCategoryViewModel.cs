namespace Nodus.API.Models.BillCategory
{
    public class BillCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsIncludedInDailyAllowance { get; set; }
    }
}
