using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.Company;
using Nodus.API.Models.Wrappers;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public CompanyController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.ManageCompanies)]
        public async Task<ActionResult<PagedResponse<List<CompanyViewModel>>>> GetCompanies([FromQuery] AllCompaniesRequestModel request)
        {
            var grpcResponse = await _jamalProtoService.GetCompaniesAsync(_mapper.Map<GetCompaniesRequest>(request));

            var companies = _mapper.Map<List<CompanyViewModel>>(grpcResponse.Companies);

            return Ok(new PagedResponse<List<CompanyViewModel>>(
                companies,
                request.Pagination.Offset,
                request.Pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpGet("{companyId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ManageCompanies)]
        public async Task<ActionResult<Response<CompanyViewModel>>> GetCompanyById([Required] int companyId)
        {
            var response = await _jamalProtoService.GetCompanyByIdAsync(_mapper.Map<GetCompanyByIdRequest>(companyId));

            return Ok(new Response<CompanyViewModel>(_mapper.Map<CompanyViewModel>(response)));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(FeatureNames.ManageCompanies)]
        public async Task<ActionResult<Response<int>>> CreateCompany([FromBody] CreateCompanyRequestModel request)
        {
            var grpcResponse = await _jamalProtoService.CreateCompanyAsync(_mapper.Map<CreateCompanyRequest>(request));

            return Ok(new Response<CreateCompanyResponseModel>(_mapper.Map<CreateCompanyResponseModel>(grpcResponse)));
        }

        [HttpDelete("{companyId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ManageCompanies)]
        public async Task<ActionResult> DeleteCompany([Required] int companyId)
        {
            await _jamalProtoService.DeleteCompanyAsync(_mapper.Map<DeleteCompanyRequest>(companyId));

            return Ok();
        }
    }
}
