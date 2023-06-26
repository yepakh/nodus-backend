namespace Nodus.API.Models.BillCategory
{
    public class CreateBillCategoryRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsIncludedInDailyAllowance { get; set; }
    }
}
