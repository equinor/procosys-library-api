using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllRegisters
{
    public class GetAllRegistersQuery : IRequest<Result<IEnumerable<RegisterDto>>>
    {
        public GetAllRegistersQuery(string plant) => Plant = plant ?? throw new ArgumentNullException(nameof(plant));

        public string Plant { get; }
    }
}
