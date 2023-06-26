using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nodus.API.CustomAttributes;
using Nodus.API.Models.Feature;
using Nodus.API.Models.Wrappers;
using Nodus.Database.Context.Constants;
using Nodus.Jamal.Service.Protos;

namespace Nodus.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeatureController : ControllerBase
    {
        private readonly Jam.JamClient _jamalProtoService;
        private readonly IMapper _mapper;

        public FeatureController(Jam.JamClient jamalProtoService, IMapper mapper)
        {
            _jamalProtoService = jamalProtoService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [AuthorizeRole(FeatureNames.WriteUsers)]
        public async Task<ActionResult<Response<List<FeatureViewModel>>>> GetFeatures()
        {
            var response = await _jamalProtoService.GetFeaturesAsync(new Empty());

            return Ok(new Response<List<FeatureViewModel>>(_mapper.Map<List<FeatureViewModel>>(response.Features)));
        }
    }
}
