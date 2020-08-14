using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.GetAllDisciplines;
using Equinor.Procosys.Library.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Library.WebApi.Controllers.Discipline
{
    [ApiController]
    [Route("Disciplines")]
    public class DisciplinesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DisciplinesController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_GENERAL_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisciplineDto>>> GetAllDisciplines(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            string plant,
            [FromQuery]
            IEnumerable<string> classifications)
        {
            var result = await _mediator.Send(new GetAllDisciplinesQuery(plant, classifications));
            return this.FromResult(result);
        }
    }
}
