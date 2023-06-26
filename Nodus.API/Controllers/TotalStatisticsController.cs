using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.SingleTripStatistics;
using Nodus.API.Models.TotalStatistics;
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
    public class TotalStatisticsController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public TotalStatisticsController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<TotalStatisticsInfoResponse>))]
        public async Task<IActionResult> GetTotalStatisticsInfo([FromQuery] TotalStatisticsFilterRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<TotalStatisticsFilterRequest>(request);
            grpcRequest.CompanyId = companyId;

            var result = await _jamalProtoService.GetTotalStatisticsInfoAsync(grpcRequest);
            return Ok(new Response<TotalStatisticsInfoResponse>(result));
        }


        [HttpGet("Trips")]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<GetTotalExpensesByTripsResponse>))]
        public async Task<IActionResult> GetTotalExpensesByTrips([FromQuery] TotalStatisticsFilterRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<TotalStatisticsFilterRequest>(request);
            grpcRequest.CompanyId = companyId;

            var result = await _jamalProtoService.GetTotalExpensesByTripsAsync(grpcRequest);
            return Ok(new Response<GetTotalExpensesByTripsResponse>(result));
        }

        [HttpGet("Categories")]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<GetTripStatisticsByCategoryResponse>))]
        public async Task<IActionResult> GetTotalExpensesByCategories([FromQuery] TotalStatisticsFilterRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<TotalStatisticsFilterRequest>(request);
            grpcRequest.CompanyId = companyId;

            var result = await _jamalProtoService.GetTotalExpensesByCategoriesAsync(grpcRequest);
            return Ok(new Response<GetTripStatisticsByCategoryResponse>(result));
        }


        [HttpGet("Users")]
        [Authorize]
        [AuthorizeRole(FeatureNames.AccessStatistics)]
        [ProducesResponseType(200, Type = typeof(Response<GetTripStatisticsByUserResponse>))]
        public async Task<IActionResult> GetTotalExpensesByUsers([FromQuery] TotalStatisticsFilterRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                    .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<TotalStatisticsFilterRequest>(request);
            grpcRequest.CompanyId = companyId;

            var result = await _jamalProtoService.GetTotalExpensesByUsersAsync(grpcRequest);
            return Ok(new Response<GetTripStatisticsByUserResponse>(result));
        }
    }
}
