using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Admin
{
    public class Company : DbEntityBase
    {
        [Column(TypeName = EntityConstants.DEFAULT_NAME_TYPE)]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_DESCRIPTION_TYPE)]
        public string Description { get; set; }

        [Column(TypeName = EntityConstants.DEFAULT_CONNECTION_STRING_TYPE)]
        [Required]
        public string ConnectionString { get; set; }

        public Company()
        {
            Roles = new HashSet<Role>();
        }
        public ICollection<Role> Roles { get; private set; }
    }
}
