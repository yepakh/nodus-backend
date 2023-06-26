using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.User;
using Nodus.API.Models.Wrappers;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyAdminController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public CompanyAdminController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.ManageCompanies)]
        public async Task<ActionResult<PagedResponse<List<CompanyAdminViewModel>>>> GetAllCompanyAdmins([FromQuery] AllCompanyAdminsRequestModel request)
        {
            var grpcResponse = await _jamalProtoService.GetAllCompanyAdminsAsync(_mapper.Map<GetAllCompanyAdminsRequest>(request));

            var admins = _mapper.Map<List<CompanyAdminViewModel>>(grpcResponse.CompanyAdmins);

            return Ok(new PagedResponse<List<CompanyAdminViewModel>>(
                admins,
                request.Pagination.Offset,
                request.Pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }
    }
}