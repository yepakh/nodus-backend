using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.Role;
using Nodus.API.Models.Wrappers;
using Nodus.Auth.Handler;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public RoleController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet("{roleId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadUsers)]
        public async Task<ActionResult<Response<RoleViewModel>>> GetRole(int roleId)
        {
            //TODO: Forbid access to roles from other companies

            var grpcRequest = _mapper.Map<GetRoleRequest>(roleId);
            var grpcResponse = await _jamalProtoService.GetRoleAsync(grpcRequest);

            var result = _mapper.Map<RoleViewModel>(grpcResponse.Role);

            return Ok(new Response<RoleViewModel>(result));
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadUsers)]
        public async Task<ActionResult<PagedResponse<List<RoleViewModel>>>> GetAllCompanyRoles([FromQuery] AllCompanyRolesRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetRolesRequest>((Request: request, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.GetRolesAsync(grpcRequest);

            var result = _mapper.Map<List<RoleViewModel>>(grpcResponse.Roles);

            return Ok(new PagedResponse<List<RoleViewModel>>(
                result,
                request.Pagination.Offset,
                request.Pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult<Response<int>>> CreateRole(CreateRoleRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            if (!FeaturesAreValid(request.AvaliableFeaturesIds))
            {
                return BadRequest("Wrong features are selected");
            }

            var grpcRequest = _mapper.Map<CreateRoleRequest>((Request: request, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.CreateRoleAsync(grpcRequest);

            return Ok(new Response<int>(grpcResponse.RoleId));
        }

        [HttpPut]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult> UpdateRole(UpdateRoleRequestModel request)
        {
            //TODO: Forbid access to roles from other companies

            if (!FeaturesAreValid(request.AvaliableFeaturesIds))
            {
                return BadRequest("Wrong features are selected");
            }

            var grpcRequest = _mapper.Map<UpdateRoleRequest>(request);
            await _jamalProtoService.UpdateRoleAsync(grpcRequest);

            return Ok();
        }

        [HttpDelete("{roleId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult> DeleteRole([Required] int roleId)
        {
            //TODO: Forbid access to roles from other companies
            //TODO: add validation not to delete assigned role (to give detailed error message)

            var grpcRequest = _mapper.Map<DeleteRoleRequest>(roleId);
            await _jamalProtoService.DeleteRoleAsync(grpcRequest);

            return Ok();
        }

        private bool FeaturesAreValid(List<int> features)
        {
            return !features.Contains((int)FeatureNames.ManageCompanies);
        }
    }
}
