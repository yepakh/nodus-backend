using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.User;
using Nodus.API.Models.Wrappers;
using Nodus.Auth.Handler;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public UserController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadUsers)]
        public async Task<ActionResult<Response<UserViewModel>>> GetUser(string userId)
        {
            var grpcResponse = await _jamalProtoService.GetUserAsync(_mapper.Map<GetUserRequest>(userId));

            return Ok(new Response<UserViewModel>(_mapper.Map<UserViewModel>(grpcResponse.User)));
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadUsers)]
        public async Task<ActionResult<Response<CreateUserResponse>>> GetAllCompanyUsers([FromQuery] AllCompanyUsersRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetAllCompanyUsersRequest>((Request: request, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.GetAllCompanyUsersAsync(grpcRequest);

            var result = _mapper.Map<List<UserViewModel>>(grpcResponse.Users);

            return Ok(new PagedResponse<List<UserViewModel>>(
                result,
                request.Pagination.Offset,
                request.Pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult<Response<Guid>>> CreateUser(CreateUserRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<CreateUserRequest>((Request: request, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.CreateUserAsync(grpcRequest);

            return Ok(new Response<CreateUserResponse>(grpcResponse));
        }

        [HttpPut]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult> UpdateUserAsync(UpdateUserRequestModel request)
        {
            await _jamalProtoService.UpdateUserAsync(_mapper.Map<UpdateUserRequest>(request));

            return Ok();
        }

        [HttpDelete("{userId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult> DisableUser([Required] string userId)
        {
            await _jamalProtoService.DisableUserAsync(_mapper.Map<DisableUserRequest>(userId));

            return Ok();
        }
    }
}
