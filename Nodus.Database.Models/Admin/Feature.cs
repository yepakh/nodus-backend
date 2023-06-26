using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Admin
{
    public class Feature : DbEntityBase
    {
        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Description { get; set; }

        public ICollection<RoleFeature> RoleFeatures { get; private set; }

        public Feature()
        {
            RoleFeatures = new HashSet<RoleFeature>();
        }
    }
}
