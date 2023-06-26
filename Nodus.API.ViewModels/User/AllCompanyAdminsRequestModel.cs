using Nodus.API.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.User
{
    public class AllCompanyAdminsRequestModel
    {
        [Required]
        public int CompanyId { get; set; }

        [Required]
        public RequestPagination Pagination { get; set; }
    }
}
