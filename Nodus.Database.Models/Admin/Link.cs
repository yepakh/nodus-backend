using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Database.Models.Admin
{
    public class Link
    {
        [Key]
        public Guid Id { get; set; }

        [Column("UserID")]
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateExpires { get; set; }

        [Required]
        public bool IsEpxired { get; set; }
    }
}
