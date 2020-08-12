using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Domain;
using Equinor.Procosys.Library.Query.GetAllResponsibles;
using Equinor.Procosys.Library.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Library.WebApi.Controllers.Responsible
{
    [ApiController]
    [Route("Responsibles")]
    public class ResponsiblesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ResponsiblesController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_GENERAL_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponsibleDto>>> GetAllResponsiblesAsync(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            string plant)
        {
            var result = await _mediator.Send(new GetAllResponsiblesQuery(plant));
            return this.FromResult(result);
        }
    }
}
