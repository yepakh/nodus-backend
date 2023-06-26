using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Company
{
    public class CreateCompanyRequestModel
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyDescription { get; set; }

        [Required]
        public string AdminFirstName { get; set; }

        [Required]
        public string AdminLastName { get; set; }

        [Required]
        public string AdminEmail { get; set; }

        [Required]
        public string AdminPhoneNumber { get; set; }

        public string ConnectionString { get; set; }
    }
}
