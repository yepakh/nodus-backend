using Microsoft.AspNetCore.Mvc;
using Nodus.API.Models.Wrappers;
using Nodus.NotificaitonService;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly Notificator.NotificatorClient _notificatorClient;

        public EmailController(Notificator.NotificatorClient notificatorClient)
        {
            _notificatorClient = notificatorClient;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Response<SendEmailResponse>>> SendEmail(EmailRequest emailRequest)
        {
            var result = await _notificatorClient.SendEmailAsync(emailRequest);
            if (result.Success)
            {
                return Ok(new Response<SendEmailResponse>(result));
            }

            return BadRequest(result.Message);
        }
    }
}
