using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.BillCategory;
using Nodus.API.Models.Common;
using Nodus.API.Models.Wrappers;
using Nodus.Auth.Handler;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;
using System.ComponentModel.DataAnnotations;

namespace Nodus.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BillCategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Jam.JamClient _jamalProtoService;

        public BillCategoryController(IMapper mapper, Jam.JamClient jamalProtoService)
        {
            _mapper = mapper;
            _jamalProtoService = jamalProtoService;
        }

        [HttpGet("{categoryId}/Statistics")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadTrips)]
        public async Task<ActionResult> GetBillCategoryStatistics([Required] int categoryId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetBillCategoryRequest>((CategoryId: categoryId, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.GetBillCategoryInfoAsync(grpcRequest);

            return Ok(new Response<BillCategoryInfoResponse>(grpcResponse));
        }


        [HttpGet("{categoryId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadTrips)]
        public async Task<ActionResult> GetBillCategory([Required] int categoryId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetBillCategoryRequest>((CategoryId: categoryId, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.GetBillCategoryAsync(grpcRequest);

            return Ok(new Response<BillCategoryViewModel>(_mapper.Map<BillCategoryViewModel>(grpcResponse.BillCategory)));
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadTrips)]
        [ProducesResponseType(200, Type = typeof(PagedResponse<List<BillCategoryViewModel>>))]
        public async Task<ActionResult> GetBillCategories([FromQuery][Required] RequestPagination pagination)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetBillCategoriesRequest>((Pagination: pagination, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.GetBillCategoriesAsync(grpcRequest);

            return Ok(new PagedResponse<List<BillCategoryViewModel>>(
                _mapper.Map<List<BillCategoryViewModel>>(grpcResponse.BillCategories),
                pagination.Offset,
                pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> CreateBillCategory([FromBody][Required] CreateBillCategoryRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<CreateBillCategoryRequest>((Request: request, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.CreateBillCategoryAsync(grpcRequest);

            return Ok(new Response<int>(grpcResponse.BillCategoryId));
        }

        [HttpPut]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> UpdateBillCategory([FromBody][Required] UpdateBillCategoryRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<UpdateBillCategoryRequest>((Request: request, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.UpdateBillCategoryAsync(grpcRequest);

            return Ok();
        }

        [HttpDelete("{categoryId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> DeleteBillCategory([Required] int categoryId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<DeleteBillCategoryRequest>((CategoryId: categoryId, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.DeleteBillCategoryAsync(grpcRequest);

            return Ok();
        }
    }
}
