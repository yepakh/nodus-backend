using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.User
{
    public class UpdateUserRequestModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Notes { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
