using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models
{
    public abstract class DbEntityBase
    {
        [Column("ID")]
        [Key]
        public int Id { get; set; }
    }
}
