using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Models.Common
{
    public class RequestPagination
    {
        [Required]
        public int Offset { get; set; }

        [Required]
        public int Limit { get; set; }
    }
}
