using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Admin
{
    public class RoleFeature
    {
        [Column("RoleID")]
        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [Column("FeatureID")]
        [Required]
        public int FeatureId { get; set; }

        [ForeignKey("FeatureId")]
        public Feature Feature { get; set; }
    }
}
