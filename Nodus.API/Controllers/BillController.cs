using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.Bill;
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
    public class BillController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Jam.JamClient _jamalProtoService;

        public BillController(IMapper mapper, Jam.JamClient jamalProtoService)
        {
            _mapper = mapper;
            _jamalProtoService = jamalProtoService;
        }

        [HttpGet("{billId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadBills)]
        public async Task<ActionResult> GetBill([Required][FromRoute] int billId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = new GetBillRequest() { BillId = billId, CompanyId = companyId };
            var grpcResponse = await _jamalProtoService.GetBillAsync(grpcRequest);

            return Ok(new Response<BillViewModel>(_mapper.Map<BillViewModel>(grpcResponse.Bill)));
        }

        [HttpGet("Trip/{tripId}/User/{userId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.ReadBills)]
        public async Task<ActionResult> GetUserBills(
            [Required][FromRoute] int tripId,
            [Required][FromRoute] Guid userId,
            [Required][FromQuery] RequestPagination pagination)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = new GetUserBillsRequest()
            {
                UserId = userId.ToString(),
                TripId = tripId,
                Pagination = _mapper.Map<PaginationRequest>(pagination),
                CompanyId = companyId
            };
            var grpcResponse = await _jamalProtoService.GetUserBillsAsync(grpcRequest);

            return Ok(new PagedResponse<List<BillViewModel>>(
                _mapper.Map<List<BillViewModel>>(grpcResponse.Bills),
                pagination.Offset,
                pagination.Limit,
                grpcResponse.Pagination.TotalCount));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteBills)]
        public async Task<ActionResult> CreateBill([Required][FromBody] CreateBillRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var userId = Guid.Parse(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.UserId).Value);

            var grpcRequest = _mapper.Map<CreateBillRequest>((Request: request, CompanyId: companyId, CreatorId: userId));
            var grpcResponse = await _jamalProtoService.CreateBillAsync(grpcRequest);

            return Ok(new Response<object>(new
            {
                grpcResponse.BillId
            }));
        }

        [HttpPut]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteBills)]
        public async Task<ActionResult> UpdateBill([Required][FromBody] UpdateBillRequestModel request)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var userId = Guid.Parse(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.UserId).Value);

            var grpcRequest = _mapper.Map<UpdateBillRequest>((Request: request, CompanyId: companyId, UpdatedById: userId));
            var grpcResponse = await _jamalProtoService.UpdateBillAsync(grpcRequest);

            return Ok();
        }

        [HttpDelete("{billId}")]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteBills)]
        public async Task<ActionResult> DeleteBill([Required] int billId)
        {
            var companyId = Convert.ToInt32(HttpContext.User.Claims
                .Single(claim => claim.Type == Constants.CompanyId).Value);

            var grpcRequest = new DeleteBillRequest() { BillId = billId, CompanyId = companyId };
            var grpcResponse = await _jamalProtoService.DeleteBillAsync(grpcRequest);

            return Ok();
        }
    }
}
