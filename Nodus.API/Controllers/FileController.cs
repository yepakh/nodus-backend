using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.Models.Wrappers;
using Nodus.Jamal.Service.Protos;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly Jam.JamClient _jamClient;

        public FileController(Jam.JamClient jamClient)
        {
            _jamClient = jamClient;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetFilePath([FromBody] GetFilePathRequest request)
        {
            var response = await _jamClient.GetFilePathAsync(request);
            return Ok(new Response<GetFilePathResponse>(response));
        }
    }
}
