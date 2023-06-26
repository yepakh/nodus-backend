using Nodus.API.Models.Feature;

namespace Nodus.API.Models.Role
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
            Features = new List<FeatureViewModel>();
        }
        public RoleViewModel(int id, string name, string description, List<FeatureViewModel> features)
        {
            Id = id;
            Name = name;
            Description = description;
            Features = features;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FeatureViewModel> Features { get; set; }
    }
}
