using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodus.Database.Models.Admin
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Column("RoleID")]
        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        [Column(TypeName = EntityConstants.DEFAULT_EMAIL_TYPE)]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? LastLogin { get; set; }

    }
}
