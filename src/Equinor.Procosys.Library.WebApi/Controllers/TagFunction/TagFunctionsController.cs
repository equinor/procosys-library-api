﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.GetTagFunctions;
using Equinor.Procosys.Library.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Library.WebApi.Controllers.TagFunction
{
    [ApiController]
    [Route("TagFunctions")]
    public class TagFunctionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagFunctionsController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_GENERAL_READ)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagFunctionDto>>> GetAllTagFunctionsAsync(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] string registerCode)
        {
            var result = await _mediator.Send(new GetTagFunctionsQuery(plant, registerCode));
            return this.FromResult(result);
        }
    }
}
