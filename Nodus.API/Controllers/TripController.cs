using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.Bill;
using Nodus.API.Models.Common;
using Nodus.API.Models.Trip;
using Nodus.API.Models.User;
using Nodus.API.Models.Wrappers;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;
using System.ComponentModel.DataAnnotations;
using Constants = Nodus.Auth.Handler.Constants;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TripController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public TripController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet("{tripId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadTrips)]
        public async Task<ActionResult> GetTrip([Required] int tripId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetTripRequest>((TripId: tripId, CompanyId: companyId));
            var grpcResponse = await _jamalProtoService.GetTripAsync(grpcRequest);

            return Ok(new Response<TripViewModel>(_mapper.Map<TripViewModel>(grpcResponse.Trip)));
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadTrips)]
        public async Task<ActionResult> GetUserTrips([FromQuery][Required] AllUserTripsRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var userId = Guid.Parse(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.UserId).Value);

            var grpcRequest = _mapper.Map<GetUserTripsRequest>((UserId: userId, CompanyId: companyId, Request: request));
            var grpcResponse = await _jamalProtoService.GetUserTripsAsync(grpcRequest);

            var mappedResponse = _mapper.Map<List<TripViewModel>>(grpcResponse.Trips);

            return Ok(new PagedResponse<List<TripViewModel>>(
                mappedResponse,
                request.Pagination.Offset,
                request.Pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpGet("[action]")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadTrips)]
        public async Task<ActionResult> GetTripUsers([FromQuery][Required] GetTripUsersRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<GetTripUsersRequest>((CompanyId: companyId, Request: request));
            var grpcResponse = await _jamalProtoService.GetTripUsersAsync(grpcRequest);

            return Ok(new PagedResponse<List<TripUserViewModel>>(
                _mapper.Map<List<TripUserViewModel>>(grpcResponse.Users),
                request.Pagination.Offset,
                request.Pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> CreateTrip([FromBody][Required] CreateTripRequestModel request)
        {
            if (request.Budget < 0 || request.Users.Any(user => user.Budget < 0 || !user.CanUploadBills && user.Budget != 0))
            {
                return BadRequest("Budget is wrong");
            }

            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var userId = Guid.Parse(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.UserId).Value);

            var grpcRequest = _mapper.Map<CreateTripRequest>((Request: request, CompanyId: companyId, CreatorId: userId));
            var grpcResponse = await _jamalProtoService.CreateTripAsync(grpcRequest);

            return Ok(new Response<object>(new
            {
                grpcResponse.TripId
            }));
        }

        [HttpPut("[action]")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> UpdateTripData([FromBody][Required] UpdateTripDataRequestModel request)
        {
            if (request.Budget < 0)
            {
                return BadRequest("Budget is wrong");
            }

            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<UpdateTripDataRequest>((Request: request, CompanyId: companyId));
            await _jamalProtoService.UpdateTripDataAsync(grpcRequest);

            return Ok();
        }

        [HttpPut("[action]")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> ChangeTripStatus([FromBody][Required] ChangeTripStatusRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<ChangeTripStatusRequest>((Request: request, CompanyId: companyId));
            await _jamalProtoService.ChangeTripStatusAsync(grpcRequest);

            return Ok();
        }

        [HttpPut("[action]")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> UpdateTripUsers([Required] AddUserToTripRequestModel request)
        {
            if (request.Users.Any(user => user.Budget < 0 || !user.CanUploadBills && user.Budget != 0))
            {
                return BadRequest("Budget is wrong");
            }

            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<UpdateTripUsersRequest>((Request: request, CompanyId: companyId));
            await _jamalProtoService.UpdateTripUsersAsync(grpcRequest);

            return Ok();
        }

        [HttpDelete("{tripId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult> DeleteTrip([Required] int tripId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = _mapper.Map<DeleteTripRequest>((TripId: tripId, CompanyId: companyId));
            await _jamalProtoService.DeleteTripAsync(grpcRequest);

            return Ok();
        }

        [HttpGet("[action]")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteTrips)]
        public async Task<ActionResult<Response<CreateUserResponse>>> GetAllCompanyUsers([FromQuery][Required] AllCompanyUsersRequestModel request)
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


        #region Bill functionality

        [HttpGet("{tripId}/Bills")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadBills)]
        public async Task<ActionResult> GetTripBills(
            [Required][FromRoute] int tripId,
            [Required][FromQuery] RequestPagination pagination)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = new GetTripBillsRequest()
            {
                TripId = tripId,
                Pagination = _mapper.Map<PaginationRequest>(pagination),
                CompanyId = companyId
            };
            var grpcResponse = await _jamalProtoService.GetTripBillsAsync(grpcRequest);

            return Ok(new PagedResponse<List<BillViewModel>>(
                _mapper.Map<List<BillViewModel>>(grpcResponse.Bills),
                pagination.Offset,
                pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        #endregion Bill functionality
    }
}
