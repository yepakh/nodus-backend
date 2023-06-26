using Nodus.API.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Company
{
    public class AllCompaniesRequestModel
    {
        [Required]
        public RequestPagination Pagination { get; set; }
    }
}
