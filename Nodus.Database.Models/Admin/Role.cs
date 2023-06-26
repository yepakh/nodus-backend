using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Admin
{
    public class Role : DbEntityBase
    {
        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Description { get; set; }

        [Column("CompanyID")]
        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public Role()
        {
            Users = new HashSet<User>();
            RoleFeatures = new HashSet<RoleFeature>();
        }

        public virtual ICollection<User> Users { get; private set; }
        public ICollection<RoleFeature> RoleFeatures { get; private set; }
    }
}
