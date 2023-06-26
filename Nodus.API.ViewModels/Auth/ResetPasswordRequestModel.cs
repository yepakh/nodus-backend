using Microsoft.AspNetCore.Mvc;

namespace Nodus.API.Models.Auth
{
    public class ResetPasswordRequestModel
    {
        [FromQuery]
        public Guid LinkId { get; set; }
        [FromQuery]
        public Guid UserId { get; set; }
        [FromBody]
        public string Password { get; set; }
    }
}
