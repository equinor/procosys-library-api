using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.GetAllAreas;
using Equinor.Procosys.Library.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Library.WebApi.Controllers.Area
{
    [ApiController]
    [Route("Areas")]
    public class AreasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AreasController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaDto>>> GetAllAreasAsync([FromHeader( Name = PlantProvider.PlantHeader)] string plant)
        {
            var result = await _mediator.Send(new GetAllAreasQuery(plant));
            return this.FromResult(result);
        }
    }
}
