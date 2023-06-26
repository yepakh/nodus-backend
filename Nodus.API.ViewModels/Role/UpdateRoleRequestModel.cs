namespace Nodus.API.Models.Role
{
    public class UpdateRoleRequestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> AvaliableFeaturesIds { get; set; }
    }
}
