using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Database.Models.Admin
{
    public class TgChat
    {
        [Column("ID")]
        [Key]
        public long Id { get; set; }

        [Column("UserID")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
