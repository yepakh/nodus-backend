namespace Nodus.API.Models.Role
{
    public class CreateRoleRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> AvaliableFeaturesIds { get; set; }
    }
}
