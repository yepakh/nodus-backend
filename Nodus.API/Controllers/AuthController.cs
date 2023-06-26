using Microsoft.AspNetCore.Mvc;
using Nodus.API.Models.Auth;
using Nodus.API.Models.Common;
using Nodus.API.Models.Wrappers;
using Nodus.Auth;
using Nodus.Auth.Handler;
using Nodus.Jamal.Service.Protos;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthGrpcClient _authGrpcService;
        private readonly Jam.JamClient _jamalProtoService;

        public AuthController(AuthGrpcClient authGrpcService, Jam.JamClient jamalProtoService)
        {
            _authGrpcService = authGrpcService;
            _jamalProtoService = jamalProtoService;
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(NotFoundResponse))]
        [HttpPost("SendResetPasswordEmail")]
        public async Task<IActionResult> SendResetPasswrodEmail([FromBody] GetUserByEmailRequest request)
        {
            var response = await _jamalProtoService.SendForgotPasswordEmailAsync(request);

            if(response.Response == String.Empty)
            {
                return Ok();
            }

            return NotFound(new NotFoundResponse(response.Response));
        }

        //create login enpoint
        [HttpPost("login")]
        public async Task<ActionResult<Response<object>>> Login([FromBody] LoginRequestModel request)
        {
            LoginUserRequest loginUserRequest = new LoginUserRequest
            {
                Email = request.Email,
                Password = request.Password
            };

            var loginResponse = await _authGrpcService.LoginUserAsync(loginUserRequest);

            var user = await _jamalProtoService.GetUserByEmailAsync(new GetUserByEmailRequest() { Email = request.Email });
            var result = new
            {
                Token = loginResponse.Token_,
                user.User
            };

            return Ok(new Response<object>(result));
        }

        //create login endpoint
        [HttpPost("resetPassword")]
        public async Task<ActionResult<string>> ResetPassword(ResetPasswordRequestModel request)
        {
            SetPasswordRequest setPasswordRequest = new SetPasswordRequest
            {
                LinkId = request.LinkId.ToString(),
                UserId = request.UserId.ToString(),
                Password = request.Password
            };
            var result = await _authGrpcService.SetPasswordAsync(setPasswordRequest);
            return Ok(result);
        }

        [HttpGet("CheckLinkIsValid")]
        public async Task<ActionResult<bool>> CheckLinkIsValid([FromQuery] Guid userId, [FromQuery] Guid linkId)
        {
            var request = new CheckLinkIsValidRequest()
            {
                LinkId = linkId.ToString(),
                UserId = userId.ToString()
            };
            var response = await _authGrpcService.CheckLinkIsValidAsync(request);
            return Ok(response);
        }


    }
}
