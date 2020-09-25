using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.GetFunctionalRoles;
using Equinor.Procosys.Library.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Library.WebApi.Controllers.FunctionalRole
{
    [ApiController]
    [Route("FunctionalRoles")]
    public class FunctionalRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FunctionalRolesController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_GENERAL_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FunctionalRoleDto>>> GetAllFunctionalRolesAsync(
            [FromHeader(Name = PlantProvider.PlantHeader)] [Required]
            string plant,
            [FromQuery] string classification)
        {
            var result = await _mediator.Send(new GetFunctionalRolesQuery(plant, classification));
            return this.FromResult(result);
        }
    }
}
