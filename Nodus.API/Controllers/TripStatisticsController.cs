using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.SingleTripStatistics;
using Nodus.API.Models.Wrappers;
using Nodus.Auth.Handler;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [CustomAttributes.AuthorizeRole(FeatureNames.AccessStatistics)]
    public class TripStatisticsController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public TripStatisticsController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet("Categories")]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<GetTripStatisticsByCategoryResponse>))]
        public async Task<IActionResult> GetTripStatisticsByCategories([FromQuery] GetTripStatisticsRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetTripStatisticsRequest>(request);
            grpcRequest.CompanyId = companyId;
            var result = await _jamalProtoService.GetTripStatisticsByCategoryAsync(grpcRequest);
            return Ok(new Response<GetTripStatisticsByCategoryResponse>(result));
        }

        [HttpGet("Users")]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<GetTripStatisticsByUserResponse>))]
        public async Task<IActionResult> GetTripStatisticsByUsers([FromQuery] GetTripStatisticsRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetTripStatisticsRequest>(request);
            grpcRequest.CompanyId = companyId;

            var result = await _jamalProtoService.GetTripStatisticsByUserAsync(grpcRequest);
            return Ok(new Response<GetTripStatisticsByUserResponse>(result));
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<GetTripStatisticsTotalBillsResponse>))]
        public async Task<IActionResult> GetTotalBillsInTripStatistic([FromQuery] GetTripStatisticsTotalBillsRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetTripStatisticsTotalBillsRequest>(request);
            grpcRequest.Filters.CompanyId = companyId;

            var result = await _jamalProtoService.GetTripStatisticsTotalBillsAsync(grpcRequest);
            return Ok(new Response<GetTripStatisticsTotalBillsResponse>(result));
        }
    }
}
