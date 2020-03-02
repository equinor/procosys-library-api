using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllAreas
{
    public class GetAllAreasQuery : IRequest<Result<IEnumerable<AreaDto>>>
    {
        public GetAllAreasQuery(string plant) => Plant = plant;

        public string Plant { get; }
    }
}
