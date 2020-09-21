using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class GetFunctionalRolesQuery : IRequest<Result<IEnumerable<FunctionalRoleDto>>>
    {
        public GetFunctionalRolesQuery(string plant, string classification)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Classification = classification;
        }

        public string Plant { get; }
        public string Classification { get; }
    }
}
